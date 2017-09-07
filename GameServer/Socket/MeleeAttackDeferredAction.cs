using System;
using GameServer.Dwarves.Persons;
using WebGame.Common.Connection;

namespace GameServer.Socket
{
    internal class MeleeAttackDeferredAction : DeferredAction
    {
        private ServerDataPerson playerPerson;
        private ServerDataPerson enemy;

        public MeleeAttackDeferredAction(long startTime, long duration, ServerDataPerson playerPerson, ServerDataPerson enemy) : base(startTime, duration)
        {
            this.playerPerson = playerPerson;
            this.enemy = enemy;
        }

        protected override void Finish()
        {
            enemy.Dead();
        }

        internal bool CanExecute()
        {
            var distance = Math.Pow(playerPerson.x - enemy.x, 2) + Math.Pow(playerPerson.y - enemy.y, 2);
            return distance < MapConst.TILE_SIZE;
        }
    }

    internal abstract class DeferredAction
    {
        public bool Execute { get; private set; }
        private long startTime;
        private long duration;
        public DeferredAction(long startTime, long duration)
        {
            this.startTime = startTime;
            this.duration = duration;
        }

        

        protected abstract void Finish();

        internal void TestFinish()
        {
            if (Execute)
                Finish();
        }

        internal virtual void Update(long deltaTime, long currentTime)
        {
            if (currentTime - startTime > duration)
                Execute = true;
        }
    }
}