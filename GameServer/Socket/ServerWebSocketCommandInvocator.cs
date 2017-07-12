using NetCoreStack.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Socket
{
    public class ServerWebSocketCommandInvocator : IServerWebSocketCommandInvocator
    {
        private readonly IConnectionManager connectionManager;
        private readonly IMainLoop mainLoop;
        public ServerWebSocketCommandInvocator(IConnectionManager connectionManager, IMainLoop mainLoop)
        {
            this.connectionManager = connectionManager;
            this.mainLoop = mainLoop;
        }

        public async Task InvokeAsync(WebSocketMessageContext context)
        {
            var connection = context.GetConnectionId();
            mainLoop.RegisterEvent(connection);
            await Task.CompletedTask;//_connectionManager.SendAsync(connection, context);
        }
    }
}
