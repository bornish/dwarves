using GameServer.Dwarves.Map;
using GameServer.Dwarves.PersonActions;
using GameServer.Socket;
using WebGame.Common.Connection;

namespace GameServer.Dwarves.Persons
{
    abstract public class ServerDataPerson : DataPerson
    {
        private const int SPEED = 4;
        public string serverSecretData = "secret";
        public DeferredAction lastDeferredAction;
        public PlayerAction CurrentAction { get; set; }

        internal void DoAction(long currentTime, MapContainer container)
        {
            if (state == PersonState.Alive)
            {
                DoPersonAction(currentTime, container);
            }
            else 
            {
                // TODO здесь анимация прерывается, надо бы на клиенте сделать обработку прерывания анимаций
                currentAnimation = null;
            }
        }

        internal abstract void DoPersonAction(long currentTime, MapContainer container);

        protected static void DoContinueAnimation(AnimationDescription currentAnimation, long currentTime)
        {
            if (currentAnimation == null || currentAnimation.end)
                return;

            currentAnimation.start = false;
            if (currentTime - currentAnimation.timeStart > currentAnimation.allDuration)
            {
                currentAnimation.end = true;
            }
        }

        internal void Dead()
        {
            state = PersonState.Dead;
        }

        protected static (float, float, Direction) Move(float x, float y, LongAction action, Direction direction, MapContainer container)
        {
            var newX = x;
            var newY = y;
            // TODO по диогонали слишком быстро
            // TODO оставить множитель для всех дел со временем
            if ((action & LongAction.GoDown) == LongAction.GoDown)
            {
                newY = y + SPEED;
                direction = Direction.Down;
            }

            if ((action & LongAction.GoUp) == LongAction.GoUp)
            {
                newY = y - SPEED;
                direction = Direction.Up;
            }

            if ((action & LongAction.GoLeft) == LongAction.GoLeft)
            {
                newX = x - SPEED;
                direction = Direction.Left;
            }

            if ((action & LongAction.GoRight) == LongAction.GoRight)
            {
                newX = x + SPEED;
                direction = Direction.Right;
            }


            if (CanMove(newX, newY, container))
            {
                return (newX, newY, direction);
            }
            return (x, y, direction);
        }

        private static bool CanMove(float newX, float newY, MapContainer container)
        {
            var tiles = container.GetTiles();

            if (newX >= MapConst.TILE_SIZE * MapConst.WIDTH || newX < 0)
                return false;

            if (newY >= MapConst.TILE_SIZE * MapConst.HEIGHT || newY < 0)
                return false;
            int i = (int)newX / MapConst.TILE_SIZE;
            int j = (int)newY / MapConst.TILE_SIZE;
            if (tiles[i, j] == TileType.Stone)
                return false;

            return true;
        }
    }
}
