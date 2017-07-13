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

        internal abstract void PressButtonW();
        internal abstract void PressButtonA();
        internal abstract void PressButtonD();
        internal abstract void PressButtonS();

        public abstract void OnMessage(float x, float y);
    }
}
