using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebGame.Js;

namespace WebGame.Js
{
    abstract class Connection
    {
        public Game Reception { get; internal set; }
        public abstract void SendData();
    }
}
