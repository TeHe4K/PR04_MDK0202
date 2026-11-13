using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        public static List<User> users = new List<User>();
        public static IPAddress IPAdress;
        public static int Port;
        static void Main(string[] args)
        {
            try
            {
                Database.Init();
                users = Database.LoadUsers();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка подключения к базе данных: " + ex.Message);
                Console.ReadKey();
                return;
            }
            Console.Write("Введите IP-адрес сервера: ");
            string ipAddressInput = Console.ReadLine();
            Console.Write("Введите порт сервера: ");
            string portInput = Console.ReadLine();
            if(int.TryParse(portInput, out Port) && IPAddress.TryParse(ipAddressInput, out IPAdress))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Сервер будет запущен на IP: {IPAdress}, Порт: {Port}");
                StartServer();
            }
            Console.Read();
        }

        private static string UploadedName(string json)
        {
            try { return JsonConvert.DeserializeObject<FileInfoFTP>(json).Name; }
            catch { return "?"; }
        }

        public static bool AutorizationUser(string name, string password)
        {
            User user = null;
            user = users.Find(u => u.name == name && u.password == password);
            return user != null;
        }

        public static bool IsInsideRoot(string root, string path)
        {
            root = Path.GetFullPath(root).TrimEnd('\\');
            path = Path.GetFullPath(path).TrimEnd('\\');
            return path.Equals(root, StringComparison.OrdinalIgnoreCase)
                || path.StartsWith(root + "\\", StringComparison.OrdinalIgnoreCase);
        }

        // Скрытые и системные элементы (например "Мои рисунки" — служебные ссылки Windows,
        // которые нельзя открыть, и desktop.ini) не показываем, как и Проводник
        public static bool IsHiddenOrSystem(string path)
        {
            FileAttributes attributes = File.GetAttributes(path);
            return (attributes & (FileAttributes.Hidden | FileAttributes.System)) != 0;
        }

        public static List<string> GetDirectory(string src)
        {
            List<string> FoldersFiles = new List<string>();
            if (Directory.Exists(src))
            {
                string[] dirs = Directory.GetDirectories(src);
                foreach (string dir in dirs)
                {
                    if (IsHiddenOrSystem(dir))
                        continue;
                    string NameDirectory = Path.GetFileName(dir);
                    FoldersFiles.Add(NameDirectory + "/");
                }
                string[] files = Directory.GetFiles(src);
                foreach (string file in files)
                {
                    if (IsHiddenOrSystem(file))
                        continue;
                    string NameFile = Path.GetFileName(file);
                    FoldersFiles.Add(NameFile);
                }
            }
            return FoldersFiles;
        }

        public static void StartServer()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAdress, Port);
            Socket sListener = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            sListener.Bind(endPoint);
            sListener.Listen(10);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Сервер запущен. Ожидание подключений...");
            while (true)
            {
                Socket Handler = null;
                try
                {
                    Handler = sListener.Accept();
                    // Читаем запрос целиком: клиент после отправки закрывает свою сторону (Shutdown),
                    // поэтому Receive вернёт 0, когда все данные получены. Так не обрезаются большие файлы.
                    string Data;
                    using (MemoryStream Received = new MemoryStream())
                    {
                        byte[] Bytes = new byte[65536];
                        int BytesRec;
                        while ((BytesRec = Handler.Receive(Bytes)) > 0)
                            Received.Write(Bytes, 0, BytesRec);
                        Data = Encoding.UTF8.GetString(Received.ToArray());
                    }
                    Console.Write("Сообщение от пользователя: " + (Data.Length > 200 ? Data.Substring(0, 200) + "..." : Data) + "\n");
                    string Reply = "";
                    ViewModelSend ViewModelSend = JsonConvert.DeserializeObject<ViewModelSend>(Data);
                    if (ViewModelSend != null)
                    {
                        VIewModelMessage ViewModelMessage;
                        string[] DataComand = ViewModelSend.Message.Split(new string[1] { " " }, StringSplitOptions.None);
                        bool known = ViewModelSend.Id >= 0 && ViewModelSend.Id < users.Count;
                        if (known && (DataComand[0] == "cd" || DataComand[0] == "get"))
                            Database.LogCommand(users[ViewModelSend.Id].dbId, ViewModelSend.Message);
                        else if (known && DataComand[0] != "connect")
                            Database.LogCommand(users[ViewModelSend.Id].dbId, "set " + UploadedName(ViewModelSend.Message));
                        if (DataComand[0] == "connect")
                        {
                            string[] DataMessage = ViewModelSend.Message.Split(new string[1] { " " }, StringSplitOptions.None);
                            if (AutorizationUser(DataMessage[1], DataMessage[2]))
                            {
                                int idUser = users.FindIndex(u => u.name == DataMessage[1] && u.password == DataMessage[2]);
                                ViewModelMessage = new VIewModelMessage("authorization", idUser.ToString());
                                Database.LogCommand(users[idUser].dbId, "connect " + DataMessage[1]);


                            }
                            else
                            {
                                ViewModelMessage = new VIewModelMessage("message", "Не верные данные");
                            }
                            Reply = JsonConvert.SerializeObject(ViewModelMessage);
                            byte[] msg = Encoding.UTF8.GetBytes(Reply);
                            Handler.Send(msg);
                        }
                        else if (DataComand[0] == "cd")
                        {
                            if (known)
                            {
                                string[] DataMessage = ViewModelSend.Message.Split(new string[1] { " " }, StringSplitOptions.None);
                                List<string> FoldersFiles = new List<string>();
                                bool found = false;
                                if (DataMessage.Length == 1)
                                {
                                    users[ViewModelSend.Id].temp_src = users[ViewModelSend.Id].src;
                                    FoldersFiles = GetDirectory(users[ViewModelSend.Id].src);
                                    found = true;
                                }
                                else
                                {
                                    string cdFolder = string.Join(" ", DataMessage.Skip(1)).Trim().Trim('"');
                                    string current = users[ViewModelSend.Id].temp_src ?? users[ViewModelSend.Id].src;
                                    string newPath = Path.IsPathRooted(cdFolder) ? cdFolder : Path.Combine(current, cdFolder);
                                    newPath = Path.GetFullPath(newPath);
                                    // Пользователь не может выйти выше своей корневой папки
                                    if (IsInsideRoot(users[ViewModelSend.Id].src, newPath) && Directory.Exists(newPath))
                                    {
                                        // Сначала читаем содержимое, и только если получилось — меняем текущую папку.
                                        // Иначе при ошибке доступа сервер "застрял" бы в папке, которую нельзя открыть.
                                        FoldersFiles = GetDirectory(newPath);
                                        users[ViewModelSend.Id].temp_src = newPath;
                                        found = true;
                                    }
                                }
                                if (!found)
                                {
                                    ViewModelMessage = new VIewModelMessage("message", "Папка не найдена");
                                }
                                else
                                {
                                    ViewModelMessage = new VIewModelMessage("cd", JsonConvert.SerializeObject(FoldersFiles));
                                }

                            }
                            else
                            {
                                ViewModelMessage = new VIewModelMessage("message", "Вы не авторизованы");
                            }
                            Reply = JsonConvert.SerializeObject(ViewModelMessage);
                            byte[] msg = Encoding.UTF8.GetBytes(Reply);
                            Handler.Send(msg);
                        }
                        else if (DataComand[0] == "get")
                        {
                            if (known)
                            {
                                string[] DataMessage = ViewModelSend.Message.Split(new string[1] { " " }, StringSplitOptions.None);
                                string filePath = "";
                                for (int i = 1; i < DataMessage.Length; i++)
                                {
                                    if (filePath == "")
                                    {
                                        filePath += DataMessage[i];
                                    }
                                    else
                                    {
                                        filePath += " " + DataMessage[i];
                                    }
                                }
                                // GetFileName отрезает пути вида "..\..\file", чтобы нельзя было скачать файл вне своей папки
                                byte[] byteFile = File.ReadAllBytes(Path.Combine(users[ViewModelSend.Id].temp_src ?? users[ViewModelSend.Id].src, Path.GetFileName(filePath)));
                                ViewModelMessage = new VIewModelMessage("file", JsonConvert.SerializeObject(byteFile));
                            }
                            else
                            {
                                ViewModelMessage = new VIewModelMessage("message", "Вы не авторизованы");
                            }
                            Reply = JsonConvert.SerializeObject(ViewModelMessage);
                            byte[] msg = Encoding.UTF8.GetBytes(Reply);
                            Handler.Send(msg);
                        }
                        else
                        {
                            if (known)
                            {
                                FileInfoFTP SendFileInfp = JsonConvert.DeserializeObject<FileInfoFTP>(ViewModelSend.Message);
                                File.WriteAllBytes(Path.Combine(users[ViewModelSend.Id].temp_src ?? users[ViewModelSend.Id].src, Path.GetFileName(SendFileInfp.Name)), SendFileInfp.Data);
                                ViewModelMessage = new VIewModelMessage("message", "Файл успешно загружен");
                            }
                            else
                                ViewModelMessage = new VIewModelMessage("message", "Вы не авторизованы");
                            Reply = JsonConvert.SerializeObject(ViewModelMessage);
                            byte[] msg = Encoding.UTF8.GetBytes(Reply);
                            Handler.Send(msg);
                        }
                    }
                } catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                    try
                    {
                        if (Handler != null)
                            Handler.Send(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new VIewModelMessage("message", "Ошибка сервера: " + ex.Message))));
                    }
                    catch { }
                }
                finally
                {
                    if (Handler != null)
                        Handler.Close();
                }
            }
        }

    }
}
