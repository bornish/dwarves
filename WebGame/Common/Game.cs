using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebGame.Common.Display;
using WebGame.Common.Types;

namespace WebGame.Common
{
    abstract class Game
    {
        protected Connection connection;
        private Person person;
        private Map map;
        public Game(Input input, Connection connection)
        {
            input.Controller = this;
            connection.Reception = this;
            this.connection = connection;
        }

        public void Init()
        {
            map = CreateMap();
            person = CreatePerson();
        }

        internal abstract Map CreateMap();
        internal abstract Person CreatePerson();
        public void OnMessage(WordlState worldState)
        {
            person.X = worldState.persons[0].x;
            person.Y = worldState.persons[0].y;

            map.Update(worldState.tiles, 20, 20);
        }

        internal void Send(string action, string param)
        {
            connection.SendData(action, param);
        }
    }
}
