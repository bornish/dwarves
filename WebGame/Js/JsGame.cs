using Bridge.Html5;
using Bridge.Pixi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebGame.Js
{
    class JsGame : Game
    {
        private Graphics graphics;
        private IRenderer app;
        private Container stage;

        public JsGame():base(new JsInput(), new Socket())
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
        }

        internal override void PressButtonA()
        {
            graphics.Position.X = graphics.Position.X - 10;
            connection.SendData();
        }

        internal override void PressButtonD()
        {
            graphics.Position.X = graphics.Position.X + 10;
            connection.SendData();
        }

        internal override void PressButtonS()
        {
            graphics.Position.Y = graphics.Position.Y - 10;
            connection.SendData();
        }

        internal override void PressButtonW()
        {
            graphics.Position.Y = graphics.Position.Y + 10;
            connection.SendData();
        }

        private void Animate()
        {
            Window.RequestAnimationFrame(Animate);
            app.Render(stage);
        }
    }
}
