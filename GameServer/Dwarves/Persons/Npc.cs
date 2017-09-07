using GameServer.Dwarves.Map;
using GameServer.Socket;

namespace GameServer.Dwarves.Persons
{
    public class NpcPerson : ServerDataPerson
    {
        internal override void DoPersonAction(long currentTime, MapContainer container)
        {
            DoContinueAnimation(currentAnimation, currentTime);
            (x, y, direction) = Move(x, y, LongAction.None, direction, container);
        }
    }
}
