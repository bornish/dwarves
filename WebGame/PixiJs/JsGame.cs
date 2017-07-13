using System;
using Bridge.Html5;
using Bridge.Pixi;
using WebGame.Common;

namespace WebGame.PixiJs
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

        public override void OnMessage(float x, float y)
        {
            graphics.Position.X = x;
            graphics.Position.Y = y;
        }

        private void Animate()
        {
            Window.RequestAnimationFrame(Animate);
            app.Render(stage);
        }
    }
}
