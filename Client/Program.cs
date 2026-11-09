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

namespace Client
{
    internal class Program
    {
        public static IPAddress iPAddress;
        public static int port;
        public static int Id = -1;
        static void Main(string[] args)
        {
            Console.Write("Введите IP-адрес сервера: ");
            string ipAddressInput = Console.ReadLine();
            Console.Write("Введите порт сервера: ");
            string portInput = Console.ReadLine();
            if (int.TryParse(portInput, out port) && IPAddress.TryParse(ipAddressInput, out iPAddress))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Данные успешно введены. Подключаюсь к серверу.");
                while (true)
                {
                    ConnectServer();
                }
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Неверный IP-адрес или порт.");
            Console.ReadKey();
        }

        public static bool CheckCommand(string msg)
        {
            bool BCommand = false;
            string[] DataMessage = msg.Split(new string[1] { " " }, StringSplitOptions.None);
            if (DataMessage.Length > 0)
            {
                string Command = DataMessage[0];
                if (Command == "connect")
                {
                    if (DataMessage.Length != 3)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Использование: connect [Login] [Password]\nПример: connect User1 P@ssw0rd");
                        BCommand = false;
                    }
                    else
                    {
                        BCommand = true;
                    }
                }
                else if (Command == "cd")
                    BCommand = true;
                else if (Command == "get")
                {
                    if (DataMessage.Length == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Использование: get [NameFile]\nПример: get Test.txt");
                        BCommand = false;
                    }
                    else
                        BCommand = true;
                }
                else if (Command == "set")
                {
                    if (DataMessage.Length == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Использование: set [NameFile]\nПример: set Test.txt");
                        BCommand = false;
                    }
                    else
                        BCommand = true;
                }
            }
            return BCommand;
        }

        public static void ConnectServer()
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(iPAddress, port);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                using (socket)
                {
                socket.Connect(endPoint);
                if (socket.Connected)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    string message = Console.ReadLine();
                    if (CheckCommand(message))
                    {
                        ViewModelSend viewModelSend = new ViewModelSend(message, Id);
                        if (message.Split(new string[1] { " " }, StringSplitOptions.None)[0] == "set")
                        {
                            string[] DataMessage = message.Split(new string[1] { " " }, StringSplitOptions.None);
                            string NameFile = "";
                            for (int i = 1; i < DataMessage.Length; i++)
                            {
                                if (NameFile == "")
                                    NameFile = DataMessage[i];
                                else
                                    NameFile += " " + DataMessage[i];
                            }
                            if (File.Exists(NameFile))
                            {
                                FileInfo FileInfo = new FileInfo(NameFile);
                                FileInfoFTP NewFileInfo = new FileInfoFTP(File.ReadAllBytes(NameFile), FileInfo.Name);
                                viewModelSend = new ViewModelSend(JsonConvert.SerializeObject(NewFileInfo), Id);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"Файл {NameFile} не найден.");
                                return;
                            }
                        }
                        byte[] messageByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(viewModelSend));
                        int BytesSend = socket.Send(messageByte);
                        byte[] bytes = new byte[10485760];
                        int BytesRec = socket.Receive(bytes);
                        string messageServer = Encoding.UTF8.GetString(bytes, 0, BytesRec);
                        VIewModelMessage vIewModelMessage = JsonConvert.DeserializeObject<VIewModelMessage>(messageServer);
                        if (vIewModelMessage.Command == "authorization")
                        {
                            Id = int.Parse(vIewModelMessage.Data);
                        }
                        else if (vIewModelMessage.Command == "message")
                            Console.WriteLine(vIewModelMessage.Data);
                        else if (vIewModelMessage.Command == "cd")
                        {
                            List<string> FoldersFiles = new List<string>();
                            FoldersFiles = JsonConvert.DeserializeObject<List<string>>(vIewModelMessage.Data);
                            foreach (string Name in FoldersFiles)
                            {
                                Console.WriteLine(Name);
                            }
                        }
                        else if (vIewModelMessage.Command == "file")
                        {
                            string[] DataMessage = viewModelSend.Message.Split(new string[1] { " " }, StringSplitOptions.None);
                            string getFile = "";
                            for (int i = 1; i < DataMessage.Length; i++)
                                if (getFile == "")
                                    getFile = DataMessage[i];
                                else
                                    getFile += " " + DataMessage[i];
                            byte[] byteFile = JsonConvert.DeserializeObject<byte[]>(vIewModelMessage.Data);
                            File.WriteAllBytes(getFile, byteFile);
                        }
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ошибка подключения к серверу.");
                }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка подключения к серверу: {ex.Message}");
            }
        }
    }
}
