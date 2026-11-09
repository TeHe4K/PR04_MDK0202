using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace Server
{
    /// <summary>
    /// Хранение пользователей и истории их команд в SQLite (файл ftp.db рядом с Server.exe).
    /// Файл базы и таблицы создаются автоматически при первом запуске.
    /// </summary>
    public static class Database
    {
        private static readonly string DbFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ftp.db");
        private static readonly string Connection = "Data Source=" + DbFile + ";Version=3;";

        public static void Init()
        {
            Execute(@"
CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Login TEXT NOT NULL UNIQUE,
    Password TEXT NOT NULL,
    Src TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS Commands (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL REFERENCES Users(Id),
    Command TEXT NOT NULL,
    Date TEXT NOT NULL DEFAULT (datetime('now','localtime'))
);
INSERT INTO Users (Login, Password, Src)
SELECT 'user1', 'password1', 'C:\Users\Public\Documents'
WHERE NOT EXISTS (SELECT 1 FROM Users);");
        }

        public static List<User> LoadUsers()
        {
            List<User> users = new List<User>();
            using (SQLiteConnection connection = new SQLiteConnection(Connection))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand("SELECT Id, Login, Password, Src FROM Users ORDER BY Id", connection);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User user = new User(reader.GetString(1), reader.GetString(2), reader.GetString(3));
                        user.dbId = reader.GetInt32(0);
                        users.Add(user);
                    }
                }
            }
            return users;
        }

        public static void LogCommand(int userDbId, string text)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(Connection))
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand("INSERT INTO Commands (UserId, Command) VALUES (@u, @c)", connection);
                    command.Parameters.AddWithValue("@u", userDbId);
                    command.Parameters.AddWithValue("@c", text);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Не удалось записать команду в БД: " + ex.Message);
            }
        }

        private static void Execute(string sql)
        {
            using (SQLiteConnection connection = new SQLiteConnection(Connection))
            {
                connection.Open();
                new SQLiteCommand(sql, connection).ExecuteNonQuery();
            }
        }
    }
}
