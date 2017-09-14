
using System;
using WebGame.Common.Connection;

namespace WebGame.Common.Display
{
    class DisplayThing
    {
        public Thing thing;
        public bool needRemove;

        public DisplayThing(Thing thing)
        {
            this.thing = thing;
        }

        internal void Remove()
        {
            throw new NotImplementedException();
        }
    }
}
