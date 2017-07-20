using System;
using Bridge.Html5;
using Bridge.jQuery2;
using WebGame.Common;

namespace WebGame.PixiJs
{
    class JsInput : Input
    {
        delegate void MyDelegate(MyObject e);

        public JsInput()
        {
            MyDelegate keyDown = new MyDelegate(OnKeyDown);
            jQuery.Document.On("keydown", keyDown);

            MyDelegate keyUp = new MyDelegate(OnKeyUp);
            jQuery.Document.On("keyup", keyUp);

            MyDelegate scrool = new MyDelegate(OnScroll);
            jQuery.Document.Bind("mousewheel DOMMouseScroll", scrool);
        }

        private void OnScroll(MyObject ev)
        {
            dynamic e = ev;
            if (e.originalEvent.wheelDelta > 0 || e.originalEvent.detail < 0)
                WheelUp();
            else
                WheelDown();
        }

        private void OnKeyDown(MyObject e)
        {
            if (e.which == 65)
                DownButtonA();
            if (e.which == 68)
                DownButtonD();
            if (e.which == 83)
                DownButtonS();
            if (e.which == 87)
                DownButtonW();
        }

        private void OnKeyUp(MyObject e)
        {
            if (e.which == 65)
                UpButtonA();
            if (e.which == 68)
                UpButtonD();
            if (e.which == 83)
                UpButtonS();
            if (e.which == 87)
                UpButtonW();
        }

        public class MyObject
        {
            public int which;
        }
    }
}
