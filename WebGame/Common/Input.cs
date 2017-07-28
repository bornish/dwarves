using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebGame.Common.Connection;

namespace WebGame.Common
{
    abstract class Input
    {
        public Game controller;

        protected void DownButtonW()
        {
            Send("StartGo", "Up");
        }

        protected void DownButtonA()
        {
            Send("StartGo", "Left");
        }

        protected void DownButtonD()
        {
            Send("StartGo", "Right");
        }

        protected void DownButtonS()
        {
            Send("StartGo", "Down");
        }

        protected void UpButtonW()
        {
            Send("StopGo", "Up");
        }

        protected void UpButtonA()
        {
            Send("StopGo", "Left");
        }

        protected void UpButtonD()
        {
            Send("StopGo", "Right");
        }

        protected void UpButtonS()
        {
            Send("StopGo", "Down");
        }

        // TODO переделать
        public void Click(long personId)
        {
            Click(ClickEventType.Player, personId);
        }

        protected void Click(ClickEventType clickEventType, long personId)
        {
            controller.SendExtend("Click", clickEventType.ToString(), personId.ToString());
        }

        protected void WheelUp()
        {
            controller.ScaleUp();
        }

        protected void WheelDown()
        {
            controller.ScaleDown();
        }

        private void Send(string action, string param)
        {
            controller.Send(action, param);
        }
    }
}
