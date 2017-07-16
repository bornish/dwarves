
namespace WebGame.Common.Types
{

    public class WordlState
    {
        public Person[] persons;
        public TileType[,] tiles;
    }

    public class Person
    {
        public float x;
        public float y;
    }

    public enum TileType
    {
        //Unkonw = 0, Empty = 1, Stone = 2
        Empty = 0, Stone = 1
    }
}
