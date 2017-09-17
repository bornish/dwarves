
using System;
using GameServer.Dwarves.Persons;
using WebGame.Common.Connection;
using GameServer.Dwarves.Map;

namespace GameServer.Dwarves.PersonActions
{
    public class DigDeferredAction : DeferredAction
    {
        private long i;
        private long j;

        public DigDeferredAction(long startTime, long duration, MapContainer container, PlayerPerson playerPerson, long i, long j) : base(playerPerson, startTime, duration, container)
        {
            this.i = i;
            this.j = j;
        }

        protected override void Finish()
        {
            container.DigTileFinish(i, j);
        }

        internal override bool CanExecute()
        {
            if (i > MapConst.WIDTH || j > MapConst.HEIGHT)
                return false;

            var type = container.GetTiles()[i, j];
            if (type == TileType.Empty)
                return false;

            var topX = (i + 1) * MapConst.TILE_SIZE;
            var downX = i * MapConst.TILE_SIZE;
            var topY = (j + 1) * MapConst.TILE_SIZE;
            var downY = j * MapConst.TILE_SIZE;
            var maxDistance = MapConst.TILE_SIZE;
            var minDistX = Math.Min(Math.Abs(ownerAction.x - topX), Math.Abs(ownerAction.x - downX));
            var minDistY = Math.Min(Math.Abs(ownerAction.y - topY), Math.Abs(ownerAction.y - downY));
            return (ownerAction.x < topX && ownerAction.x > downX && minDistY < maxDistance)
                || (ownerAction.y < topY && ownerAction.y > downY && minDistX < maxDistance);
        }
    }
}
