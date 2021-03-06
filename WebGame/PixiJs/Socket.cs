﻿using System;
using Bridge.Html5;
using WebGame.Common;
using WebGame.Common.Connection;

namespace WebGame.PixiJs
{
    class Socket : Connect
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
                        SendData(RequestPlayerAction.Registration, login, null);

                    }
                    else if (data.Command == 2)
                    {
                        WordlState state = data.Value;
                        reception.OnMessage(state);
                    }
                }
            };
        }

        public override void SendData(RequestPlayerAction action, string param1, string param2)
        {
            if (socket == null || socket.ReadyState != WebSocket.State.Open)
            {
                Window.Alert("socket not connected");
            }
            var context = new { header = new { connectionId = connectId }, value = new { action = action, param1 = param1, param2 = param2 } };
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
