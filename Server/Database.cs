using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Server
{
    /// <summary>
    /// Хранение пользователей и истории их команд в SQL Server LocalDB.
    /// База и таблицы создаются автоматически при первом запуске.
    /// </summary>
    public static class Database
    {
        private const string DbName = "FtpDb";
        private const string MasterConnection = @"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Initial Catalog=master;";
        private const string Connection = @"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Initial Catalog=" + DbName + ";";

        public static void Init()
        {
            Execute(MasterConnection,
                "IF DB_ID('" + DbName + "') IS NULL CREATE DATABASE [" + DbName + "]");

            Execute(Connection, @"
IF OBJECT_ID('Users') IS NULL
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Login NVARCHAR(100) NOT NULL UNIQUE,
        Password NVARCHAR(100) NOT NULL,
        Src NVARCHAR(500) NOT NULL
    );
IF OBJECT_ID('Commands') IS NULL
    CREATE TABLE Commands (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL REFERENCES Users(Id),
        Command NVARCHAR(1000) NOT NULL,
        Date DATETIME NOT NULL DEFAULT GETDATE()
    );
IF NOT EXISTS (SELECT 1 FROM Users)
    INSERT INTO Users (Login, Password, Src) VALUES
        ('user1', 'password1', 'C:\Users\Public\Documents');");
        }

        public static List<User> LoadUsers()
        {
            List<User> users = new List<User>();
            using (SqlConnection connection = new SqlConnection(Connection))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT Id, Login, Password, Src FROM Users ORDER BY Id", connection);
                using (SqlDataReader reader = command.ExecuteReader())
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
                using (SqlConnection connection = new SqlConnection(Connection))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("INSERT INTO Commands (UserId, Command) VALUES (@u, @c)", connection);
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

        private static void Execute(string connectionString, string sql)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                new SqlCommand(sql, connection).ExecuteNonQuery();
            }
        }
    }
}
