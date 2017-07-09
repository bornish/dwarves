using NetCoreStack.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Socket
{
    public class ServerWebSocketCommandInvocator : IServerWebSocketCommandInvocator
    {
        private readonly IConnectionManager _connectionManager;
        public ServerWebSocketCommandInvocator(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public async Task InvokeAsync(WebSocketMessageContext context)
        {
            var connection = context.GetConnectionId();
            await _connectionManager.SendAsync(connection, context);
            // Sending incoming data from Backend zone to the Clients (Browsers)
        }
    }
}
