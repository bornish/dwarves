
namespace WebGame.Common.Connection
{

    public class WordlState
    {
        public DataPerson[] players;
        public DataPerson[] npc;
        public TileType[,] tiles;
        public long myId;
        public long timestamp;
    }

    public class DataPerson
    {
        public long id;
        public float x;
        public float y;
    }

    public enum TileType
    {
        //Unkonw = 0, Empty = 1, Stone = 2
        Empty = 0, Stone = 1
    }
}
