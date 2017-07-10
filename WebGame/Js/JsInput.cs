using Bridge.jQuery2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebGame.Js
{
    class JsInput : Input
    {
        delegate void MyDelegate(MyObject e);

        public JsInput()
        {
            MyDelegate myDelegate = new MyDelegate(OnKeyPress2);
            jQuery.Document.On("keypress", myDelegate);
        }

        private void OnKeyPress2(MyObject e)
        {
            if (e.which == 97)
                PressButtonA();
            if (e.which == 100)
                PressButtonD();
            if (e.which == 119)
                PressButtonS();
            if (e.which == 115)
                PressButtonW();

        }

        public class MyObject
        {
            public int which;
        }
    }
}
