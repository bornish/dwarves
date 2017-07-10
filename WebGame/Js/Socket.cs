using Bridge.Html5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebGame.Js
{
    class Socket : Connection
    {

        private string connectId;
        private WebSocket socket;

        public Socket()
        {
            var scheme = Document.Location.Protocol == "https:" ? "wss" : "ws";
            var port = !string.IsNullOrEmpty(Document.Location.Port) ? (":" + Document.Location.Port) : "";
            var url = scheme + "://" + Document.Location.HostName + port + "/ws";

            socket = new WebSocket(url)
            {
                //socket.OnOpen = 

                OnMessage = (ev) =>
                {
                    EventObject data = JSON.Parse<EventObject>((string)ev.Data);
                    if (data.Command == 4)
                    {
                        connectId = data.Value;
                    }
                }
            };
        }

        public override void SendData()
        {
            if (socket == null || socket.ReadyState != WebSocket.State.Open)
            {
                Window.Alert("socket not connected");
            }
            var context = new { header = new { connectionId = connectId }, value = "test" };
            var str = JSON.Stringify(context);
            socket.Send(str);
        }
    }

    public class EventObject
    {
        public int Command { get; set; }
        public string Value { get; set; }
    }
}
