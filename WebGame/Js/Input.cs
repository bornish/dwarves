using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebGame.Js;

namespace WebGame.Js
{
    abstract class Input
    {
        public Game Controller { get; internal set; }

        protected void PressButtonW()
        {
            Controller.PressButtonW();
        }

        protected void PressButtonA()
        {
            Controller.PressButtonA();
        }

        protected void PressButtonD()
        {
            Controller.PressButtonD();
        }

        protected void PressButtonS()
        {
            Controller.PressButtonS();
        }
    }
}
