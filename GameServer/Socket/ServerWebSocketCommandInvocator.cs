using NetCoreStack.WebSockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebGame.Common.Connection;

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
            var action = (RequestPlayerAction) Enum.Parse(typeof(RequestPlayerAction), data.GetValue("action").Value<string>());
            var param1 = data.GetValue("param1").Value<string>();
            var param2 = data.GetValue("param2").Value<string>();
            mainLoop.RegisterEvent(connection, action, param1, param2);
            await Task.CompletedTask;//_connectionManager.SendAsync(connection, context);
        }
    }
}
