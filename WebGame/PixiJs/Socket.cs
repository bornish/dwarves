using Bridge.Html5;
using WebGame.Common;
using WebGame.Common.Types;

namespace WebGame.PixiJs
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
                    //Window.Alert((string)ev.Data);
                    dynamic data = JSON.Parse<EventObject>((string)ev.Data);
                    if (data.Command == 4)
                    {
                        connectId = (string)data.Value;
                    }
                    else if (data.Command == 2)
                    {
                        WordlState state = data.Value;
                        Reception.OnMessage(state);
                    }
                }
            };
        }

        public override void SendData(string action, string param)
        {
            if (socket == null || socket.ReadyState != WebSocket.State.Open)
            {
                Window.Alert("socket not connected");
            }
            var context = new { header = new { connectionId = connectId }, value = new { action = action, param = param } };
            var str = JSON.Stringify(context);
            socket.Send(str);
        }
    }

    public class EventObject
    {
        public int Command { get; set; }
        public object Value { get; set; }
    }

    public class DataObject
    {
        public float x;
        public float y;
    }
}
