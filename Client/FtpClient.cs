using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    /// <summary>
    /// Работа с сервером: одна команда = одно TCP-подключение
    /// (отправили ViewModelSend, получили VIewModelMessage, соединение закрылось).
    /// </summary>
    public class FtpClient
    {
        public IPAddress Ip { get; set; }
        public int Port { get; set; }
        public int Id { get; private set; } = -1;

        public bool IsConnected
        {
            get { return Id != -1; }
        }

        /// <summary>Авторизация: connect [login] [password]. Сервер возвращает Id пользователя.</summary>
        public void Connect(string login, string password)
        {
            Id = -1;
            VIewModelMessage answer = Send("connect " + login + " " + password);
            if (answer.Command != "authorization")
                throw new Exception(answer.Data);
            Id = int.Parse(answer.Data);
        }

        /// <summary>
        /// Переход в папку. folder = null — корневая папка пользователя,
        /// ".." — на уровень выше, "." — обновить текущую.
        /// Возвращает список: папки с "/" на конце, затем файлы.
        /// </summary>
        public List<string> Cd(string folder)
        {
            VIewModelMessage answer = Send(folder == null ? "cd" : "cd " + folder);
            if (answer.Command != "cd")
                throw new Exception(answer.Data);
            return JsonConvert.DeserializeObject<List<string>>(answer.Data);
        }

        /// <summary>Скачивание файла из текущей папки на сервере.</summary>
        public byte[] Get(string fileName)
        {
            VIewModelMessage answer = Send("get " + fileName);
            if (answer.Command != "file")
                throw new Exception(answer.Data);
            return JsonConvert.DeserializeObject<byte[]>(answer.Data);
        }

        /// <summary>Загрузка локального файла в текущую папку на сервере.</summary>
        public string Upload(string localPath)
        {
            FileInfoFTP fileInfo = new FileInfoFTP(File.ReadAllBytes(localPath), Path.GetFileName(localPath));
            VIewModelMessage answer = Send(JsonConvert.SerializeObject(fileInfo));
            return answer.Data;
        }

        private VIewModelMessage Send(string message)
        {
            ViewModelSend viewModelSend = new ViewModelSend(message, Id);
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Connect(new IPEndPoint(Ip, Port));
                socket.Send(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(viewModelSend)));
                // Сообщаем серверу, что запрос отправлен полностью
                socket.Shutdown(SocketShutdown.Send);

                // Читаем ответ до конца (сервер закрывает соединение после ответа)
                using (MemoryStream received = new MemoryStream())
                {
                    byte[] buffer = new byte[65536];
                    int count;
                    while ((count = socket.Receive(buffer)) > 0)
                        received.Write(buffer, 0, count);
                    string json = Encoding.UTF8.GetString(received.ToArray());
                    VIewModelMessage answer = JsonConvert.DeserializeObject<VIewModelMessage>(json);
                    if (answer == null)
                        throw new Exception("Сервер не вернул ответ");
                    return answer;
                }
            }
        }
    }
}
