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
        private readonly IConnectionManager _connectionManager;
        public ServerWebSocketCommandInvocator(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public async Task InvokeAsync(WebSocketMessageContext context)
        {
            var connection = context.GetConnectionId();
            var data = (JObject)context.Value;
            var x = data.GetValue("x").Value<float>();
            var y = data.GetValue("y").Value<float>();
            if (x > 30)
                x = 30;
            if (y > 30)
                y = 30;

            var answer = new WebSocketMessageContext();
            answer.Command = WebSocketCommands.DataSend;
            answer.Value = new { X = x, Y = y };
            await _connectionManager.SendAsync(connection, answer);
            // Sending incoming data from Backend zone to the Clients (Browsers)
        }
    }

}
