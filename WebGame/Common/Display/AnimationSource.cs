using System;
using System.Collections.Generic;
using WebGame.Common.Connection;

namespace WebGame.Common.Display
{
    class AnimationSource
    {
        private Dictionary<Direction, Action<HumanPerson, float>> animations;
        private Dictionary<Direction, Action<HumanPerson>> clearAnimations;

        public AnimationSource(Action<HumanPerson, float> left, Action<HumanPerson> leftClear, Action<HumanPerson, float> up, Action<HumanPerson> upClear,
            Action<HumanPerson, float> right, Action<HumanPerson> rightClear, Action<HumanPerson, float> down, Action<HumanPerson> downClear)
        {
            animations = new Dictionary<Direction, Action<HumanPerson, float>>
            {
                [Direction.Left] = left,
                [Direction.Up] = up,
                [Direction.Right] = right,
                [Direction.Down] = down
            };

            clearAnimations = new Dictionary<Direction, Action<HumanPerson>>
            {
                [Direction.Left] = leftClear,
                [Direction.Up] = upClear,
                [Direction.Right] = rightClear,
                [Direction.Down] = downClear
            };
        }

        internal Action<HumanPerson, float> DoFor(Direction oldDirection)
        {
            return animations[oldDirection];
        }

        internal Action<HumanPerson> ClearFor(Direction oldDirection)
        {
            return clearAnimations[oldDirection];
        }
    }

    static class Spline
    {
        internal static float Calc(float start, float end, float t)
        {
            return start + (end - start) * t;
        }
    }
}
