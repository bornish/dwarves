using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebGame.Common
{
    abstract class Input
    {
        public Game Controller { get; internal set; }

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

        private void Send(string action, string param)
        {
            Controller.Send(action, param);
        }
    }
}
