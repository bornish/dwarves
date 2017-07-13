using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebGame.Common
{
    abstract class Connection
    {
        public Game Reception { get; internal set; }
        public abstract void SendData(float x, float y);
    }
}
