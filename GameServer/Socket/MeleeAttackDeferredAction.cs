using System;

namespace GameServer.Socket
{
    internal class MeleeAttackDeferredAction : DeferredAction
    {
        public MeleeAttackDeferredAction(long startTime, long duration) : base (startTime, duration)
        {
        }

        protected override void Finish()
        {
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