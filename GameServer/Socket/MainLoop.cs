using NetCoreStack.WebSockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebGame.Common.Connection;

namespace GameServer.Socket
{
    public class MainLoop : IMainLoop
    {
        private const int SPEED = 4;
        private const int DELAY_TIME = 40;

        private volatile int currentPersonId;
        private long currentNpcId;
        private long lastCurentTime;
        private IConnectionManager connectionManager;
        private ConcurrentDictionary<string, string> loginedUsers = new ConcurrentDictionary<string, string>();
        private ConcurrentDictionary<string, int> connectionDictionary = new ConcurrentDictionary<string, int>();
        private ConcurrentDictionary<int, PlayerAction> playersActions = new ConcurrentDictionary<int, PlayerAction>();

        private Dictionary<int, ServerDataPerson> playersData = new Dictionary<int, ServerDataPerson>();
        private Dictionary<long, ServerDataPerson> npcData = new Dictionary<long, ServerDataPerson>();
        private TileType[,] tiles;

        private List<DeferredAction> deferredActions = new List<DeferredAction>();  // события будут выполняться в несколько потоков в нем

        public MainLoop(IConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager;
            tiles = new TileType[MapConst.WIDTH, MapConst.HEIGHT];

            tiles[5, 5] = TileType.Stone;
            tiles[1, 1] = TileType.Stone;
            tiles[1, 0] = TileType.Stone;

            var npc = new ServerDataPerson() { x = 0f, y = 0f, id = GetNextNpcId() };
            npcData.Add(npc.id, npc);

            var timer = new Timer(LoopAsync, null, 100, Timeout.Infinite);
        }

        private long GetNextNpcId()
        {
            currentNpcId += 1;
            return currentNpcId;
        }

        private (float, float, Direction) Move(float x, float y, LongAction action, Direction direction)
        {
            var newX = x;
            var newY = y;
            // TODO по диогонали слишком быстро
            // TODO оставить множитель для всех дел со временем
            if ((action & LongAction.GoDown) == LongAction.GoDown)
            {
                newY = y + SPEED;
                direction = Direction.Down;
            }

            if ((action & LongAction.GoLeft) == LongAction.GoLeft)
            {
                newX = x - SPEED;
                direction = Direction.Left;
            }

            if ((action & LongAction.GoRight) == LongAction.GoRight)
            {
                newX = x + SPEED;
                direction = Direction.Right;
            }

            if ((action & LongAction.GoUp) == LongAction.GoUp)
            {
                newY = y - SPEED;
                direction = Direction.Up;
            }

            if (CanMove(newX, newY, tiles))
            {
                return (newX, newY, direction);
            }
            return (x, y, direction);
        }

        private async void LoopAsync(object empty)
        {
            try
            {
                lastCurentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                while (true)
                {
                    // запоминаем время начала
                    // делаем дела, думаю в один поток
                    var currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                    var deltaTime = currentTime - lastCurentTime;
                    lastCurentTime = currentTime;


                    foreach (var playerAction in playersActions)
                    {
                        ServerDataPerson data;
                        if (playersData.ContainsKey(playerAction.Key))
                        {
                            data = playersData[playerAction.Key];
                        }
                        else
                        {
                            data = new ServerDataPerson() { x = 0f, y = 0f, id = playerAction.Key };
                            playersData[playerAction.Key] = data;
                        }
                        var action = playerAction.Value.LongAction;

                        DoContinueAnimation(data.currentAnimation, currentTime);
                        (data.x, data.y, data.direction) = Move(data.x, data.y, action, data.direction);

                        if (playerAction.Value.FastAction != null && !playerAction.Value.FastAction.processed)
                        {
                            playerAction.Value.FastAction.processed = true;
                            data.currentAnimation = new AnimationDescription(AnimationNames.Attack, 500, currentTime);

                            // TODO надо сделать общий массив для всех персонажей. боты это не боты, targetId пока не трогаем,
                            //playerAction.Value.FastAction.targetId;

                            RegisterDeferredAction(new MeleeAttackDeferredAction(currentTime, 500));
                        }
                    }

                    foreach (var data in npcData.Values)
                    {
                        DoContinueAnimation(data.currentAnimation, currentTime);
                        (data.x, data.y, data.direction) = Move(data.x, data.y, LongAction.None, data.direction);
                    }

                    // TODO делать в несколько потоков
                    foreach (var action in deferredActions)
                        action.Update(deltaTime, currentTime);

                    // опять в один поток
                    foreach (var action in deferredActions)
                        action.TestFinish();

                    // TODO удалить лишние элементы, посмотреть как это сделать в c#
                    //deferredActions.Remove all Execute

                    // отправляем сообщения
                    foreach (var connection in connectionManager.Connections)
                    {
                        if (!connectionDictionary.ContainsKey(connection.Key))
                            continue;
                        var id = connectionDictionary[connection.Key];
                        var playerData = playersData[id];
                        var answer = new WebSocketMessageContext();
                        answer.Command = WebSocketCommands.DataSend;
                        var state = new WordlState()
                        {
                            players = new DataPerson[playersData.Count],
                            npc = new DataPerson[npcData.Count],
                            myId = id,
                            tiles = tiles
                        };

                        var i = 0;
                        foreach (var data in playersData.Values)
                        {
                            state.players[i] = data;
                            i++;
                        }

                        i = 0;
                        foreach (var data in npcData.Values)
                        {
                            state.npc[i] = data;
                            i++;
                        }
                        state.timestamp = currentTime;
                        answer.Value = state;
                        // TODO надо отправлять текущее время еще.
                        connectionManager.SendAsync(connection.Key, answer);
                    }
                    //TODO запоминаем время конца
                    // спим
                    await Task.Delay(DELAY_TIME);
                }
            }
            catch (Exception e)
            {
                Console.Write("Критическая ошибка");
                Console.Write(e);
            }
        }

        private void RegisterDeferredAction(DeferredAction deferredAction)
        {
            deferredActions.Add(deferredAction);
        }

        private void DoContinueAnimation(AnimationDescription currentAnimation, long currentTime)
        {
            if (currentAnimation == null || currentAnimation.end)
                return;

            currentAnimation.start = false;
            if (currentTime - currentAnimation.timeStart > currentAnimation.duration)
            {
                currentAnimation.end = true;
                currentAnimation.t = 1;
            }
            else
            {
                currentAnimation.t = (float)(currentTime - currentAnimation.timeStart) / currentAnimation.duration;
            }
            
        }

        private bool CanMove(float newX, float newY, TileType[,] tiles)
        {
            if (newX >= MapConst.TILE_SIZE * MapConst.WIDTH || newX < 0)
                return false;

            if (newY >= MapConst.TILE_SIZE * MapConst.HEIGHT || newY < 0)
                return false;
            int i = (int)newX / MapConst.TILE_SIZE;
            int j = (int)newY / MapConst.TILE_SIZE;
            if (tiles[i, j] == TileType.Stone)
                return false;

            return true;
        }

        // TODO Action должен быть enum
        public void RegisterEvent(string connection, string action, string param1, string param2)
        {
            if (action == "Registration")
            {
                var login = param1;
                var id = GetNextPersonId();
                connectionDictionary[connection] = id;
                playersActions[id] = new PlayerAction(LongAction.None, null);
            }
            else
            {
                var id = connectionDictionary[connection];
                var lastLongAction = playersActions[id].LongAction;
                var fastAction = playersActions[id].FastAction;

                if (action == "StartGo")
                {
                    if (param1 == "Right")
                        lastLongAction = lastLongAction | LongAction.GoRight;
                    else if (param1 == "Left")
                        lastLongAction = lastLongAction | LongAction.GoLeft;
                    else if (param1 == "Up")
                        lastLongAction = lastLongAction | LongAction.GoUp;
                    else if (param1 == "Down")
                        lastLongAction = lastLongAction | LongAction.GoDown;
                }
                else if (action == "StopGo")
                {
                    if (param1 == "Right")
                        lastLongAction = lastLongAction & ~LongAction.GoRight;
                    else if (param1 == "Left")
                        lastLongAction = lastLongAction & ~LongAction.GoLeft;
                    else if (param1 == "Up")
                        lastLongAction = lastLongAction & ~LongAction.GoUp;
                    else if (param1 == "Down")
                        lastLongAction = lastLongAction & ~LongAction.GoDown;
                }
                else if (action == "Click")
                {
                    fastAction = new FastAction()
                    {
                        targetId = long.Parse(param2),
                        type = FastActionType.Attack
                    };
                }
                playersActions[id] = new PlayerAction(lastLongAction, fastAction);
            }
        }

        private int GetNextPersonId()
        {
            currentPersonId += 1;
            return currentPersonId;
        }
    }

    // TODO вернуть структуры, помнить timestamp когда получили команду, в data помним timestamp последеней выполенной команды
    public class PlayerAction
    {
        public LongAction LongAction { get; }
        public FastAction FastAction { get; }

        public PlayerAction(LongAction action, FastAction fastAction)
        {
            LongAction = action;
            FastAction = fastAction;
        }
    }

    public enum LongAction
    {
        None = 0, GoRight = 1, GoLeft = 2, GoUp = 4, GoDown = 8,
    }

    public class FastAction
    {
        public FastActionType type;
        public bool processed;

        // Пока не трогаем, когда надо будет хранить больше данных, внутри структуры будет ссылка на dataAction класс, в котором храняться данные именно для этого действия
        public long targetId;

        public FastAction()
        {
            processed = false;
        }
    }

    public enum FastActionType
    {
        Attack
    }


    public class ServerDataPerson : DataPerson
    {
        public string serverSecretData = "secret";
    }

    public interface IMainLoop
    {
        void RegisterEvent(string connection, string action, string param1, string param2);
    }
}
