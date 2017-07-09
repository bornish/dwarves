using System;
using Bridge.Html5;
using Bridge.Pixi;
using Bridge.jQuery2;

namespace WebGame
{
    public class Loader
    {
        private static Graphics graphics;
        private static IRenderer app;
        private static Container stage;

        delegate void MyDelegate(MyObject e);

        public static void Main(string[] args)
        {
            app = Pixi.AutoDetectRenderer(800, 600);
            Document.Body.AppendChild(app.View);

            stage = new Container();
            graphics = new Graphics();

            // set a fill and line style
            graphics.BeginFill(0xFF3300);
            graphics.LineStyle(4, 0xffd900, 1);

            // draw a circle, set the lineStyle to zero so the circle doesn't have an outline
            graphics.LineStyle(0);
            graphics.BeginFill(0xFFFF0B, 0.5f);
            graphics.DrawCircle(470, 90, 60);
            graphics.EndFill();

            stage.AddChild(graphics);
            Animate();

            MyDelegate myDelegate = new MyDelegate(OnKeyPress2);
            jQuery.Document.On("keypress", myDelegate);
        }

        private static void Animate()
        {
            Window.RequestAnimationFrame(Animate);
            app.Render(stage);
        }

        private static void OnKeyPress2(MyObject e)
        {
            if (e.Which == 97)
                graphics.Position.X = graphics.Position.X - 10;
            if (e.Which == 100)
                graphics.Position.X = graphics.Position.X + 10;
            if (e.Which == 119)
                graphics.Position.Y = graphics.Position.Y - 10;
            if (e.Which == 115)
                graphics.Position.Y = graphics.Position.Y + 10;
        }
    }

    public class MyObject
    {
        public int Which;
    }
}
