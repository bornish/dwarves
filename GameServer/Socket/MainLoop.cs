using NetCoreStack.WebSockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebGame.Common.Types;

namespace GameServer.Socket
{
    public class MainLoop : IMainLoop
    {
        private const int width = 100;
        private const int widthLength = 20;
        private const int heightLenght = 20;

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
            tiles = new TileType[widthLength, heightLenght];

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

        private (float, float) Move(float x, float y, ActionState action)
        {
            var newX = x;
            var newY = y;
            // TODO по диогонали слишком быстро
            if ((action & ActionState.GoDown) == ActionState.GoDown)
                newY = y + 1;

            if ((action & ActionState.GoUp) == ActionState.GoUp)
                newY = y - 1;

            if ((action & ActionState.GoLeft) == ActionState.GoLeft)
                newX = x - 1;

            if ((action & ActionState.GoRight) == ActionState.GoRight)
                newX = x + 1;

            if (CanMove(newX, newY, tiles))
            {
                return (newX, newY);
            }
            return (x, y);
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
                        var action = playerAction.Value.Action;

                        (playerData.x, playerData.y) = Move(playerData.x, playerData.y, action);
                    }

                    foreach (var npc in npcData.Values)
                    {
                        (npc.x, npc.y) = Move(npc.x, npc.y, ActionState.GoDown);
                    }

                    // отправляем сообщения
                    foreach (var connection in connectionManager.Connections)
                    {
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

                        answer.Value = state;
                        connectionManager.SendAsync(connection.Key, answer);
                    }
                    // запоминаем время конца
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
            if (newX >= width * widthLength || newX < 0)
                return false;

            if (newY >= width * heightLenght || newY < 0)
                return false;
            int i = (int)newX / width;
            int j = (int)newY / width;
            if (tiles[i, j] == TileType.Stone)
                return false;

            return true;
        }

        public void RegisterEvent(string connection, string action, string param)
        {
            if (action == "Registration")
            {
                var login = param;
                var id = GetNextPersonId();
                connectionDictionary[connection] = id;
                playersActions[id] = new PlayerAction(ActionState.None);
            }
            else
            {
                var id = connectionDictionary[connection];
                var lastAction = playersActions[id].Action;

                if (action == "StartGo")
                {
                    if (param == "Right")
                        lastAction = lastAction | ActionState.GoRight;
                    else if (param == "Left")
                        lastAction = lastAction | ActionState.GoLeft;
                    else if (param == "Up")
                        lastAction = lastAction | ActionState.GoUp;
                    else if (param == "Down")
                        lastAction = lastAction | ActionState.GoDown;
                }
                else if (action == "StopGo")
                {
                    if (param == "Right")
                        lastAction = lastAction & ~ActionState.GoRight;
                    else if (param == "Left")
                        lastAction = lastAction & ~ActionState.GoLeft;
                    else if (param == "Up")
                        lastAction = lastAction & ~ActionState.GoUp;
                    else if (param == "Down")
                        lastAction = lastAction & ~ActionState.GoDown;
                }
                playersActions[id] = new PlayerAction(lastAction);
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
        public ActionState Action { get; }

        public PlayerAction(ActionState action)
        {
            Action = action;
        }
    }

    public enum ActionState
    {
        None = 0, GoRight = 1, GoLeft = 2, GoUp = 4, GoDown = 8,
    }

    public class ServerDataPerson : DataPerson
    {
        public string serverSecretData = "secret";
    }

    public interface IMainLoop
    {
        void RegisterEvent(string connection, string action, string param);
    }
}
