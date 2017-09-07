using System;
using GameServer.Dwarves.Persons;
using GameServer.Socket;
using WebGame.Common.Connection;

namespace GameServer.Dwarves.PersonActions
{
    internal class MeleeAttackDeferredAction : DeferredAction
    {
        private ServerDataPerson enemy;

        public MeleeAttackDeferredAction(long startTime, long duration, ServerDataPerson ownerAction, ServerDataPerson enemy) : base(ownerAction, startTime, duration)
        {
            this.enemy = enemy;
        }

        protected override void Finish()
        {
            enemy.Dead();
        }

        internal override bool CanExecute()
        {
            var distance = Math.Pow(ownerAction.x - enemy.x, 2) + Math.Pow(ownerAction.y - enemy.y, 2);
            return distance < MapConst.TILE_SIZE;
        }
    }

    public abstract class DeferredAction
    {
        public bool Execute { get; protected set; }
        public bool Canceled { get; protected set; }
        protected long startTime;
        protected long duration;
        protected ServerDataPerson ownerAction;

        public DeferredAction(ServerDataPerson ownerAction, long startTime, long duration)
        {
            this.ownerAction = ownerAction;
            this.startTime = startTime;
            this.duration = duration;
        }

        internal abstract bool CanExecute();
        protected abstract void Finish();

        internal void TestFinish()
        {
            if (Execute && !Canceled)
                Finish();
        }

        internal virtual void Update(long deltaTime, long currentTime)
        {
            if (currentTime - startTime > duration)
                Execute = true;
        }

        internal virtual bool PreventExecute(LongAction longAction)
        {
            return longAction != LongAction.None;
        }

        internal virtual bool PreventNew()
        {
            return !Execute;
        }

        internal virtual void MoveTryCancel()
        {
            if (!Execute)
            {
                Execute = true;
                Canceled = true;
                ownerAction.lastDeferredAction = null;

                // TODO отменяется анимация. надо проверить как на это отреагирует клиент
                ownerAction.currentAnimation = null;
            }
        }
    }
}