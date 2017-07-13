using NetCoreStack.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Socket
{
    public class MainLoop : IMainLoop
    {
        private IConnectionManager connectionManager;

        public MainLoop(IConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager;

            var timer = new Timer(LoopAsync, null, 100, Timeout.Infinite);
        }

        private async void LoopAsync(object state)
        {
            try
            {
                while (true)
                {
                    // запоминаем время начала
                    // делаем дела
                    // отправляем сообщения
                    foreach (var connection in connectionManager.Connections)
                    {
                        var context = new WebSocketMessageContext();
                        context.Command = WebSocketCommands.DataSend;
                        connectionManager.SendAsync(connection.Key, context);
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

        public void RegisterEvent(string connection)
        {
            
        }
    }

    public interface IMainLoop
    {
        void RegisterEvent(string connection);
    }
}
