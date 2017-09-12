using System;
using System.Collections.Generic;
using WebGame.Common.Display;
using WebGame.Common.Connection;

namespace WebGame.Common
{
    abstract class Game
    {
        protected Connect connection;
        private Dictionary<long, Person> persons = new Dictionary<long, Person>();
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
            foreach (var person in persons.Values)
            {
                var changeX = (person.needX - person.lastX) * delta / (currentServerTime - lastServerTime);
                var changeY = (person.needY - person.lastY) * delta / (currentServerTime - lastServerTime);
                person.X += changeX;
                person.Y += changeY;
                person.UpdateAnimation(currentTime);
            }

            camera.SetPersonPosition(persons[myId].X, persons[myId].Y);
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
            foreach (var person in worldState.persons)
            {
                UpdateStatePerson(person, persons, currentTime);
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
                if (players[person.id].currentAnimation != null)
                    players[person.id].ClearAnimation();

                // начинаем новую анимацию
                if (person.currentAnimation.longAnimation)
                    players[person.id].currentAnimation = new AnimationDescription(person.currentAnimation.name, GetLong(person.currentAnimation.allDuration), GetLong(person.currentAnimation.smallDuration), currentTime);
                else
                    players[person.id].currentAnimation = new AnimationDescription(person.currentAnimation.name, GetLong(person.currentAnimation.allDuration), currentTime); ;
            }
            if (person.currentAnimation == null && players[person.id].currentAnimation != null)
            {
                players[person.id].ClearAnimation();
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

        internal void SendData(RequestPlayerAction action, string param)
        {
            connection.SendData(action, param, null);
        }

        internal void SendData(RequestPlayerAction action, string param1, string param2)
        {
            connection.SendData(action, param1, param2);
        }

        
    }
}
