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
                    ViewModelSend viewModelSend = JsonConvert.DeserializeObject<ViewModelSend>(Data);
                    if(ViewModelSend != null)
                    {
                        VIewModelMessage vIewModelMessage;
                        string[] DataComand = ViewModelSend.Message.Split(new string[1] { " " }, StringSplitOptions.None);
                        if (DataComand[0] == "connect")
                        {
                            string[] DataMessage = ViewModelSend.Message.Split(new string[1] {" "}, StringSplitOptions.None)
                        }
                    }
                }
            }
    }
}
