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
            Send(RequestPlayerAction.StartGo, "Up");
        }

        protected void DownButtonA()
        {
            Send(RequestPlayerAction.StartGo, "Left");
        }

        protected void DownButtonD()
        {
            Send(RequestPlayerAction.StartGo, "Right");
        }

        protected void DownButtonS()
        {
            Send(RequestPlayerAction.StartGo, "Down");
        }

        protected void UpButtonW()
        {
            Send(RequestPlayerAction.StopGo, "Up");
        }

        protected void UpButtonA()
        {
            Send(RequestPlayerAction.StopGo, "Left");
        }

        protected void UpButtonD()
        {
            Send(RequestPlayerAction.StopGo, "Right");
        }

        protected void UpButtonS()
        {
            Send(RequestPlayerAction.StopGo, "Down");
        }

        internal void ClickOnLandPosition(float x, float y)
        {
            if (x >= MapConst.TILE_SIZE * MapConst.WIDTH || x < 0)
                return;

            if (y >= MapConst.TILE_SIZE * MapConst.HEIGHT || y < 0)
                return;

            int i = (int)x / MapConst.TILE_SIZE;
            int j = (int)y / MapConst.TILE_SIZE;
            controller.SendData(RequestPlayerAction.Dig, i.ToString(), j.ToString());
        }

        // TODO переделать
        public void AttackPerson(long personId)
        {
            controller.SendData(RequestPlayerAction.Attack, personId.ToString());
        }

        protected void WheelUp()
        {
            controller.ScaleUp();
        }

        protected void WheelDown()
        {
            controller.ScaleDown();
        }

        private void Send(RequestPlayerAction action, string param)
        {
            controller.SendData(action, param);
        }
    }
}
