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
            users.Add(new User("user1", "password1", @"C:\Users\Public\Documents"));
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

        public static bool AutorizationUser(string name, string password)
        {
            User user = null;
            user = users.Find(u => u.name == name && u.password == password);
            return user != null;
        }

        public static List<string> GetDirectory(string src)
        {
            List<string> FoldersFiles = new List<string>();
            if (Directory.Exists(src))
            {
                string[] dirs = Directory.GetDirectories(src);
                foreach (string dir in dirs)
                {
                    string NameDirectory = dir.Replace(src, "");
                    FoldersFiles.Add(NameDirectory + "/");
                }
                string[] files = Directory.GetFiles(src);
                foreach (string file in files)
                {
                    string NameFile = file.Replace(src, "");
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
                try
                {
                    Socket Handler = sListener.Accept();
                    string Data = null;
                    byte[] Bytes = new byte[1048760];
                    int BytesRec = Handler.Receive(Bytes);
                    Data += Encoding.UTF8.GetString(Bytes, 0, BytesRec);
                    Console.Write("Сообщение от пользователя: " + Data + "\n");
                    string Reply = "";
                    ViewModelSend ViewModelSend = JsonConvert.DeserializeObject<ViewModelSend>(Data);
                    if (ViewModelSend != null)
                    {
                        VIewModelMessage ViewModelMessage;
                        string[] DataComand = ViewModelSend.Message.Split(new string[1] { " " }, StringSplitOptions.None);
                        if (DataComand[0] == "connect")
                        {
                            string[] DataMessage = ViewModelSend.Message.Split(new string[1] { " " }, StringSplitOptions.None);
                            if (AutorizationUser(DataMessage[1], DataMessage[2]))
                            {
                                int idUser = users.FindIndex(u => u.name == DataMessage[1] && u.password == DataMessage[2]);
                                ViewModelMessage = new VIewModelMessage("authorization", idUser.ToString());


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
                            if (ViewModelSend.Id != -1)
                            {
                                string[] DataMessage = ViewModelSend.Message.Split(new string[1] { " " }, StringSplitOptions.None);
                                List<string> FoldersFiles = new List<string>();
                                if (DataMessage.Length == 1)
                                {
                                    users[ViewModelSend.Id].temp_src = users[ViewModelSend.Id].src;
                                    FoldersFiles = GetDirectory(users[ViewModelSend.Id].src);
                                }
                                else
                                {
                                    string cdFolder = "";
                                    for (int i = 1; i < DataMessage.Length; i++)
                                    {
                                        if (cdFolder == "")
                                        {
                                            cdFolder += DataMessage[i];
                                        }
                                        else
                                        {
                                            cdFolder += " " + DataMessage[i];
                                        }
                                        users[ViewModelSend.Id].temp_src = users[ViewModelSend.Id].temp_src + cdFolder;
                                        FoldersFiles = GetDirectory(users[ViewModelSend.Id].temp_src);
                                    }
                                }
                                if (FoldersFiles.Count == 0)
                                {
                                    ViewModelMessage = new VIewModelMessage("message", "Папка не найдена или пуста");
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
                            if (ViewModelSend.Id != -1)
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
                                byte[] byteFile = File.ReadAllBytes(users[ViewModelSend.Id].temp_src + filePath);
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
                            if (ViewModelSend.Id != -1)
                            {
                                FileInfoFTP SendFileInfp = JsonConvert.DeserializeObject<FileInfoFTP>(ViewModelSend.Message);
                                File.WriteAllBytes(users[ViewModelSend.Id].temp_src + @"\" + SendFileInfp.Name, SendFileInfp.Data);
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
                }
            }
        }

    }
}
