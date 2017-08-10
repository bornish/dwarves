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

        private volatile int currentPersonId;
        private long currentNpcId;
        private IConnectionManager connectionManager;
        private ConcurrentDictionary<string, string> loginedUsers = new ConcurrentDictionary<string, string>();
        private ConcurrentDictionary<string, int> connectionDictionary = new ConcurrentDictionary<string, int>();
        private ConcurrentDictionary<int, PlayerAction> playersActions = new ConcurrentDictionary<int, PlayerAction>();

        private Dictionary<int, ServerDataPerson> playersData = new Dictionary<int, ServerDataPerson>();
        private Dictionary<long, ServerDataPerson> npcData = new Dictionary<long, ServerDataPerson>();
        private TileType[,] tiles;

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
                while (true)
                {
                    // запоминаем время начала
                    // делаем дела
                    foreach (var playerAction in playersActions)
                    {
                        ServerDataPerson playerData;
                        if (playersData.ContainsKey(playerAction.Key))
                        {
                            playerData = playersData[playerAction.Key];
                        }
                        else
                        {
                            playerData = new ServerDataPerson() { x = 0f, y = 0f, id = playerAction.Key };
                            playersData[playerAction.Key] = playerData;
                        }
                        var action = playerAction.Value.LongAction;

                        (playerData.x, playerData.y, playerData.direction) = Move(playerData.x, playerData.y, action, playerData.direction);
                    }

                    foreach (var npc in npcData.Values)
                    {
                        (npc.x, npc.y, npc.direction) = Move(npc.x, npc.y, LongAction.GoDown, npc.direction);
                    }

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
                        state.timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        answer.Value = state;
                        // TODO надо отправлять текущее время еще.
                        connectionManager.SendAsync(connection.Key, answer);
                    }
                    //TODO запоминаем время конца
                    // спим
                    await Task.Delay(40);
                }
            }
            catch (Exception e)
            {
                Console.Write("Критическая ошибка");
                Console.Write(e);
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
                var attackInfo = playersActions[id].AttackInfo;

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
                    lastLongAction = lastLongAction | LongAction.Attack;
                    attackInfo = new AttackInfo(long.Parse(param2));
                }
                playersActions[id] = new PlayerAction(lastLongAction, attackInfo);
            }
        }

        private int GetNextPersonId()
        {
            currentPersonId += 1;
            return currentPersonId;
        }
    }

    public struct PlayerAction
    {
        public LongAction LongAction { get; }
        public AttackInfo? AttackInfo { get; }

        public PlayerAction(LongAction action, AttackInfo? attackInfo)
        {
            LongAction = action;
            AttackInfo = attackInfo;
        }
    }

    public enum LongAction
    {
        None = 0, GoRight = 1, GoLeft = 2, GoUp = 4, GoDown = 8, Attack = 16,
    }

    public struct AttackInfo
    {
        public long TargetId { get; }

        public AttackInfo(long targetId)
        {
            TargetId = targetId;
        }
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
