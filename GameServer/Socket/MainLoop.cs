using GameServer.Dwarves.Map;
using GameServer.Dwarves.Persons;
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
        private const int DELAY_TIME = 40;

        // TODO не самая красивая вещь, круто было бы переделать
        private volatile int currentPersonId;
        private long lastCurentTime;
        private IConnectionManager connectionManager;
        private ConcurrentDictionary<string, string> loginedUsers = new ConcurrentDictionary<string, string>();
        private ConcurrentDictionary<string, int> connectionDictionary = new ConcurrentDictionary<string, int>();
        private ConcurrentDictionary<int, PlayerAction> playersActions = new ConcurrentDictionary<int, PlayerAction>();

        private MapContainer container = new MapContainer();

        public MainLoop(IConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager;

            container.GenerateTiles();

            var npc = new NpcPerson() { x = 0f, y = 0f, id = GetNextPersonId() };

            var persons = container.getPersons();
            persons.Add(npc.id, npc);

            var timer = new Timer(LoopAsync, null, 100, Timeout.Infinite);
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

                    var persons = container.getPersons();
                    var deferredActions = container.getActions();
                    var tiles = container.GetTiles();

                    foreach (var playerAction in playersActions)
                    {
                        ServerDataPerson data;
                        if (persons.ContainsKey(playerAction.Key))
                        {
                            data = persons[playerAction.Key];
                        }
                        else
                        {
                            data = new PlayerPerson() { x = 0f, y = 0f, id = playerAction.Key };
                            persons[playerAction.Key] = data;
                        }

                        data.CurrentAction = playerAction.Value;
                    }

                    foreach (var person in persons.Values)
                    {
                        person.DoAction(currentTime, container);
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
                        var playerData = persons[id];
                        var answer = new WebSocketMessageContext();
                        answer.Command = WebSocketCommands.DataSend;
                        var state = new WordlState()
                        {
                            persons = new DataPerson[persons.Count],
                            myId = id,
                            tiles = tiles
                        };

                        var i = 0;
                        foreach (var data in persons.Values)
                        {
                            state.persons[i] = data;
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

        // TODO Action должен быть enum
        public void RegisterEvent(string connection, RequestPlayerAction action, string param1, string param2)
        {
            if (action == RequestPlayerAction.Registration)
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

                if (action == RequestPlayerAction.StartGo)
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
                else if (action == RequestPlayerAction.StopGo)
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
                else if (action == RequestPlayerAction.Attack)
                {
                    fastAction = new FastAction()
                    {
                        targetId = long.Parse(param1),
                        type = FastActionType.Attack
                    };
                }
                else if (action == RequestPlayerAction.Dig)
                {
                    fastAction = new FastAction()
                    {
                        i = long.Parse(param1),
                        j = long.Parse(param2),
                        type = FastActionType.Dig
                    };
                }
                playersActions[id] = new PlayerAction(lastLongAction, fastAction);
            }
        }

        private int GetNextPersonId()
        {
            int id = ++currentPersonId;
            return id;
        }
    }

    
    public struct PlayerAction
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

    // TODO если будет желание вернуть структуры, помнить timestamp когда получили команду, в data помним timestamp последеней выполенной команды
    public class FastAction
    {
        public FastActionType type;
        public bool processed;

        // Пока не трогаем, когда надо будет хранить больше данных, внутри структуры будет ссылка на dataAction класс, в котором храняться данные именно для этого действия
        public long targetId;
        public long i;
        public long j;
    }

    public enum FastActionType
    {
        Attack, Dig
    }

    public interface IMainLoop
    {
        void RegisterEvent(string connection, RequestPlayerAction action, string param1, string param2);
    }
}
