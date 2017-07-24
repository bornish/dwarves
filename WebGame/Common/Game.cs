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
        private Dictionary<long, Person> players = new Dictionary<long, Person>();
        private Dictionary<long, Person> npc = new Dictionary<long, Person>();
        private Map map;
        protected Camera camera;
        public Game(Input input, Connection connection)
        {
            input.controller = this;
            connection.reception = this;
            this.connection = connection;
            
        }

        protected void Init(Camera camera)
        {
            map = CreateMap();
            this.camera = camera;
        }

        internal abstract Map CreateMap();
        internal abstract Person CreatePerson();
        public void OnMessage(WordlState worldState)
        {

            // TODO удалять элементы тоже можно
            foreach (var person in worldState.players)
            {
                if (!players.ContainsKey(person.id))
                    players[person.id] = CreatePerson();
                players[person.id].X = person.x;
                players[person.id].Y = person.y;
            }

            foreach (var person in worldState.npc)
            {
                if (!npc.ContainsKey(person.id))
                    npc[person.id] = CreatePerson();
                npc[person.id].X = person.x;
                npc[person.id].Y = person.y;
            }

            map.Update(worldState.tiles, 20, 20);

            camera.SetPersonPosition(players[worldState.myId].X, players[worldState.myId].Y);
        }

        internal void ScaleUp()
        {
            camera.ScaleUp();
        }

        internal void ScaleDown()
        {
            camera.ScaleDown();
        }

        internal void Send(string action, string param)
        {
            connection.SendData(action, param);
        }
    }
}
