using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebGame.Common.Display
{
    interface IPerson
    {
        float X { get; set; }
        float Y { get; set; }
    }

    public abstract class Person : IPerson
    {
        public long id;
        public float needX;
        public float needY;
        public float lastX;
        public float lastY;

        public Person(long id)
        {
            this.id = id;
        }

        public abstract float X { get; set; }
        public abstract float Y { get; set; }
    }
}
