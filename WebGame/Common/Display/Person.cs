using System;
using WebGame.Common.Connection;

namespace WebGame.Common.Display
{
    public abstract class Person
    {
        public long id;
        public float needX;
        public float needY;
        public float lastX;
        public float lastY;
        protected Direction oldDirection;

        public Person(long id)
        {
            this.id = id;
        }

        public abstract float X { get; set; }
        public abstract float Y { get; set; }
        public abstract void SetDirection(Direction direction);

        public abstract void UpdateAnimation();
    }
}
