using System;
using System.Collections.Generic;
using WebGame.Common.Display;
using WebGame.Common.Connection;

namespace WebGame.Common
{
    abstract class Game
    {
        protected Connect connection;
        private Dictionary<long, Person> players = new Dictionary<long, Person>();
        private Dictionary<long, Person> npc = new Dictionary<long, Person>();
        private Map map;
        protected Camera camera;
        protected Input input;

        private long currentServerTime = 0;
        private long lastServerTime = 0;
        private long myId = -1;

        public Game(Input input, Connect connection)
        {
            this.input = input;
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
        internal abstract Person CreatePerson(long id);

        protected void UpdateWorld()
        {
            if (0 > myId)
                return;

            var currentTime = DateTimeNow();
            // TODO по настоящему считать прошедшее время
            float delta = 10;
            foreach (var person in players.Values)
            {
                var changeX = (person.needX - person.lastX) * delta / (currentServerTime - lastServerTime);
                var changeY = (person.needY - person.lastY) * delta / (currentServerTime - lastServerTime);
                person.X += changeX;
                person.Y += changeY;
                person.UpdateAnimation(currentTime);
            }

            foreach (var person in npc.Values)
            {
                var changeX = (person.needX - person.lastX) * delta / (currentServerTime - lastServerTime);
                var changeY = (person.needY - person.lastY) * delta / (currentServerTime - lastServerTime);
                person.X += changeX;
                person.Y += changeY;
                person.UpdateAnimation(currentTime);
            }

            camera.SetPersonPosition(players[myId].X, players[myId].Y);
        }

        public void OnMessage(WordlState worldState)
        {
            var currentTime = DateTimeNow();
            dynamic temp = worldState.timestamp;
            long temp2 = temp;
            if (temp2 < lastServerTime)
                return;

            lastServerTime = currentServerTime;
            currentServerTime = temp2;
            // TODO удалять элементы тоже можно
            foreach (var person in worldState.players)
            {
                UpdateStatePerson(person, players, currentTime);
            }

            foreach (var person in worldState.npc)
            {
                UpdateStatePerson(person, npc, currentTime);
            }

            myId = worldState.myId;
            map.Update(worldState.tiles, MapConst.WIDTH, MapConst.HEIGHT);
        }

        internal abstract long DateTimeNow();

        private void UpdateStatePerson(DataPerson person, Dictionary<long, Person> players, long currentTime)
        {
            if (!players.ContainsKey(person.id))
            {
                players[person.id] = CreatePerson(person.id);
                players[person.id].X = person.x;
                players[person.id].Y = person.y;
            }
            players[person.id].SetDirection(person.direction);
            SetNeedPosition(players[person.id], person.x, person.y);
            if (person.currentAnimation != null && person.currentAnimation.start)
            {
                // начинаем новую анимацию
                var currentAnimation = new AnimationDescription(person.currentAnimation.name, GetLong(person.currentAnimation.duration), currentTime);
                players[person.id].currentAnimation = currentAnimation;
            }
            
        }

        internal abstract long GetLong(long duration);

        private void SetNeedPosition(Person person, float x, float y)
        {
            person.lastX = person.X;
            person.lastY = person.Y;
            person.needX = x;
            person.needY = y;
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
            connection.SendData(action, param, null);
        }

        internal void SendExtend(string action, string param1, string param2)
        {
            connection.SendData(action, param1, param2);
        }

        
    }
}
