using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class User
    {
        public string name { get; set; }
        public string password { get; set; }
        public string src { get; set; }
        public string temp_src { get; set; }
        public User(string name, string password, string src)
        {
            this.name = name;
            this.password = password;
            this.src = src;
        }
    }
}
