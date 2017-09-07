using GameServer.Dwarves.Map;
using GameServer.Socket;
using System.Collections.Generic;
using WebGame.Common.Connection;

namespace GameServer.Dwarves.Persons
{
    public class PlayerPerson : ServerDataPerson
    {
        internal override void DoPersonAction(long currentTime, MapContainer container)
        {
            var action = CurrentAction.LongAction;

            DoContinueAnimation(currentAnimation, currentTime);
            (x, y, direction) = Move(x, y, action, direction, container);

            if (CurrentAction.FastAction != null && !CurrentAction.FastAction.processed)
            {
                var fastAction = CurrentAction.FastAction;
                fastAction.processed = true;

                if (fastAction.type == FastActionType.Attack)
                {
                    var id = CurrentAction.FastAction.targetId;
                    var enemy = container.FindPerson(id);

                    var meleeAttackDeferredAction = new MeleeAttackDeferredAction(currentTime, 500, this, enemy);
                    if (meleeAttackDeferredAction.CanExecute())
                    {
                        currentAnimation = new AnimationDescription(AnimationNames.Attack, 500, currentTime);
                        container.AddAction(meleeAttackDeferredAction);
                    }
                }
            }
        }
    }
}
