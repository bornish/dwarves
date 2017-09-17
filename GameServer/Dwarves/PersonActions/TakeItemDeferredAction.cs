using System;
using GameServer.Dwarves.Persons;
using GameServer.Dwarves.Map;
using WebGame.Common.Connection;

namespace GameServer.Dwarves.PersonActions
{
    public class TakeItemDeferredAction : DeferredAction
    {
        private Item item;

        public TakeItemDeferredAction(ServerDataPerson ownerAction, long startTime, long duration, MapContainer container, string guid) : base(ownerAction, startTime, duration, container)
        {
            var items = container.GetItems();
            if (items.ContainsKey(guid))
                item = items[guid]; 
        }

        protected override void Finish()
        {
            var items = container.GetItems();
            if (items.ContainsKey(item.guid))
            {
                items.Remove(item.guid);
                ownerAction.TakeItem(item);
            }
        }

        internal override bool CanExecute()
        {
            if (item == null)
                return false;
            int i = (int)ownerAction.x / MapConst.TILE_SIZE;
            int j = (int)ownerAction.y / MapConst.TILE_SIZE;

            return item.i == i && Math.Abs(item.j - j) < 2 || item.j == j && Math.Abs(item.i - i) < 2;
        }
    }
}
