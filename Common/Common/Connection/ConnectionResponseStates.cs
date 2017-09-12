
namespace WebGame.Common.Connection
{

    public class WordlState
    {
        public DataPerson[] persons;
        public TileType[,] tiles;
        public long myId;
        public long timestamp;
    }

    public class DataPerson
    {
        public long id;
        public float x;
        public float y;
        public Direction direction;
        public AnimationDescription currentAnimation;
        public PersonState state;
    }

    public class AnimationDescription
    {
        public AnimationNames name;
        public long timeStart;
        public long allDuration;
        public long smallDuration;
        public bool longAnimation;
        public bool start;
        public bool end;

        public AnimationDescription() { }
        public AnimationDescription(AnimationNames name, long allDuration, long currentTime)
        {
            this.name = name;
            this.allDuration = allDuration;
            timeStart = currentTime;
            longAnimation = false;
            start = true;
        }

        public AnimationDescription(AnimationNames name, long allDuration, long smallDuration, long currentTime)
        {
            this.name = name;
            this.allDuration = allDuration;
            this.smallDuration = smallDuration;
            timeStart = currentTime;
            longAnimation = true;
            start = true;
        }
    }

    public enum AnimationNames
    {
        Attack = 0, Dig = 1
    }

    public enum Direction
    {
        Left = 0, Up = 1, Right = 2, Down = 3,
    }
    public enum TileType
    {
        //Unkonw = 0, Empty = 1, Stone = 2
        Empty = 0, Stone = 1
    }

    public enum PersonState
    {
        Alive = 0, Dead = 1
    }
}
