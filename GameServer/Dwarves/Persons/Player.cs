using GameServer.Dwarves.Map;
using GameServer.Dwarves.PersonActions;
using GameServer.Socket;
using WebGame.Common.Connection;

namespace GameServer.Dwarves.Persons
{
    public class PlayerPerson : ServerDataPerson
    {
        internal override void DoPersonAction(long currentTime, MapContainer container)
        {
            var longAction = CurrentAction.LongAction;

            if (longAction != LongAction.None)
                lastDeferredAction?.MoveTryCancel();

            DoContinueAnimation(currentAnimation, currentTime);

            (x, y, direction) = Move(x, y, longAction, direction, container);

            if (CurrentAction.FastAction != null && !CurrentAction.FastAction.processed)
            {
                var fastAction = CurrentAction.FastAction;
                fastAction.processed = true;

                if (fastAction.type == FastActionType.Attack)
                {
                    if (id != CurrentAction.FastAction.targetId)
                    {
                        var enemy = container.FindPerson(CurrentAction.FastAction.targetId);
                        if (ExecuteAction(new MeleeAttackDeferredAction(currentTime, 500, this, enemy, container), container, longAction))
                        {
                            currentAnimation = new AnimationDescription(AnimationNames.Attack, 500, currentTime);
                        }
                    }
                }
                else if (fastAction.type == FastActionType.Dig)
                {

                    var deferredAction = new DigDeferredAction(currentTime, 5000, container, this, CurrentAction.FastAction.i, CurrentAction.FastAction.j);
                    if (ExecuteAction(deferredAction, container, longAction))
                    {
                        currentAnimation = new AnimationDescription(AnimationNames.Dig, 5000, 300, currentTime);
                    }
                }
                else if (fastAction.type == FastActionType.Take)
                {

                    var deferredAction = new TakeItemDeferredAction(this, currentTime, 0, container, CurrentAction.FastAction.guid);
                    ExecuteAction(deferredAction, container, longAction);
                }
            }
        }

        protected bool ExecuteAction(DeferredAction action, MapContainer container, LongAction longAction)
        {
            if (action.PreventExecute(longAction))
                return false;

            if (lastDeferredAction != null && lastDeferredAction.PreventNew())
                return false;

            if (action.CanExecute())
            {
                lastDeferredAction = action;
                container.AddAction(action);
                return true;
            }
            return false;
        }
    }
}
