using NetCoreStack.WebSockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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


        private IConnectionManager connectionManager;
        private ConcurrentDictionary<string, string> logins = new ConcurrentDictionary<string, string>();
        private ConcurrentDictionary<string, PlayerAction> playersActions = new ConcurrentDictionary<string, PlayerAction>();
        private Dictionary<string, PlayerData> playersData = new Dictionary<string, PlayerData>();
        private TileType[,] tiles;

        public MainLoop(IConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager;
            tiles = new TileType[widthLength, heightLenght];

            tiles[5, 5] = TileType.Stone;
            tiles[1, 1] = TileType.Stone;
            tiles[1, 0] = TileType.Stone;

            var timer = new Timer(LoopAsync, null, 100, Timeout.Infinite);
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
                        PlayerData playerData;
                        if (playersData.ContainsKey(playerAction.Key))
                        {
                            playerData = playersData[playerAction.Key];
                        }
                        else
                        {
                            playerData = new PlayerData() { x = 0f, y = 0f };
                            playersData[playerAction.Key] = playerData;
                        }
                        var action = playerAction.Value.Action;
                        var newX = playerData.x;
                        var newY = playerData.y;
                        // TODO по диогонали слишком быстро
                        if ((action & ActionState.GoDown) == ActionState.GoDown)
                            newY = playerData.y + 1;

                        if ((action & ActionState.GoUp) == ActionState.GoUp)
                            newY = playerData.y - 1;

                        if ((action & ActionState.GoLeft) == ActionState.GoLeft)
                            newX = playerData.x - 1;

                        if ((action & ActionState.GoRight) == ActionState.GoRight)
                            newX = playerData.x + 1;

                        if (CanMove(newX, newY, tiles))
                        {
                            playerData.x = newX;
                            playerData.y = newY;
                        }
                    }

                    // отправляем сообщения
                    foreach (var connection in connectionManager.Connections)
                    {

                        var playerData = playersData[logins[connection.Key]];
                        var answer = new WebSocketMessageContext();
                        answer.Command = WebSocketCommands.DataSend;
                        var state = new WordlState()
                        {
                            persons = new Person[1],
                            tiles = tiles
                        };
                        state.persons[0] = new Person() { x = playerData.x, y = playerData.y };
                        
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
            if (newX > width * widthLength || newX < 0)
                return false;

            if (newY > width * heightLenght || newY < 0)
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
                logins[connection] = login;
                playersActions[login] = new PlayerAction { Action = ActionState.None };
            }
            else
            {
                var login = logins[connection];
                var lastAction = playersActions[login].Action;

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
                playersActions[login] = new PlayerAction { Action = lastAction };
            }
        }
    }

    public struct PlayerAction
    {
        public ActionState Action { get; set; }
    }

    public enum ActionState
    {
        None = 0, GoRight = 1, GoLeft = 2, GoUp = 4, GoDown = 8,
    }

    public class PlayerData
    {
        public float x;
        public float y;
    }

    public interface IMainLoop
    {
        void RegisterEvent(string connection, string action, string param);
    }
}
