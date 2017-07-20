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
            person = CreatePerson();
            this.camera = camera;
        }

        internal abstract Map CreateMap();
        internal abstract Person CreatePerson();
        public void OnMessage(WordlState worldState)
        {
            person.X = worldState.persons[0].x;
            person.Y = worldState.persons[0].y;

            map.Update(worldState.tiles, 20, 20);

            camera.SetPersonPosition(person.X, person.Y);
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
