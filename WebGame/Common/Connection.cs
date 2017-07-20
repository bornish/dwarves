using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebGame.Common
{
    abstract class Connection
    {
        public Game reception;
        protected bool isRegister;
        protected string login = "test";

        public abstract void SendData(string action, string param);
        
    }
}
