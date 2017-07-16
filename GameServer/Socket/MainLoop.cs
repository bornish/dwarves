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
        private IConnectionManager connectionManager;
        private ConcurrentDictionary<string, PlayerAction> playersActions = new ConcurrentDictionary<string, PlayerAction>();
        private Dictionary<string, PlayerData> playersData = new Dictionary<string, PlayerData>();

        public MainLoop(IConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager;

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

                        // TODO по диогонали слишком быстро
                        if ((action & ActionState.GoDown) == ActionState.GoDown)
                            playerData.y = playerData.y + 1;

                        if ((action & ActionState.GoUp) == ActionState.GoUp)
                            playerData.y = playerData.y - 1;

                        if ((action & ActionState.GoLeft) == ActionState.GoLeft)
                            playerData.x = playerData.x - 1;

                        if ((action & ActionState.GoRight) == ActionState.GoRight)
                            playerData.x = playerData.x + 1;

                        if (playerData.x > 30)
                            playerData.x = 30;
                        if (playerData.y > 30)
                            playerData.y = 30;
                    }

                    // отправляем сообщения
                    foreach (var connection in connectionManager.Connections)
                    {
                        var playerData = playersData[connection.Key];
                        var answer = new WebSocketMessageContext();
                        answer.Command = WebSocketCommands.DataSend;
                        var state = new WordlState()
                        {
                            persons = new Person[1],
                            tiles = new TileType[20,20]
                        };
                        state.persons[0] = new Person() { x = playerData.x, y = playerData.y };
                        state.tiles[5, 5] = TileType.Stone;
                        state.tiles[1, 1] = TileType.Stone;
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

        public void RegisterEvent(string connection, string action, string param)
        {
            ActionState lastAction = ActionState.None;
            if (playersActions.ContainsKey(connection))
                lastAction = playersActions[connection].Action;

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
            playersActions[connection] = new PlayerAction { Action = lastAction };
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
