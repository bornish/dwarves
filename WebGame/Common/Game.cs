using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebGame.Common
{
    abstract class Game
    {
        protected Connection connection;
        public Game(Input input, Connection connection)
        {
            input.Controller = this;
            connection.Reception = this;
            this.connection = connection;
        }

        public abstract void OnMessage(float x, float y);

        internal void Send(string action, string param)
        {
            connection.SendData(action, param);
        }
    }
}
