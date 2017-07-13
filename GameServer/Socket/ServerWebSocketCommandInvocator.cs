using NetCoreStack.WebSockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            var data = (JObject)context.Value;
            var action = data.GetValue("action").Value<string>();
            var param = data.GetValue("param").Value<string>();
            mainLoop.RegisterEvent(connection, action, param);
            await Task.CompletedTask;//_connectionManager.SendAsync(connection, context);
        }
    }
}
