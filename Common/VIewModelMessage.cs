using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class VIewModelMessage
    {
        public string Command { get; set; }
        public string Data { get; set; }
        public ViewModelMessage(string command, string data)
        {
            Command = command;
            Data = data;
        }
    }
}
