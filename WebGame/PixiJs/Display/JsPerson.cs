using Bridge.Pixi;
using WebGame.Common.Display;

namespace WebGame.PixiJs.Display
{
    class JsPerson : Person
    {
        private Graphics graphics;

        public float X { get { return graphics.Position.X; } set { graphics.Position.X = value; } }
        public float Y { get { return graphics.Position.Y; } set { graphics.Position.Y = value; } }

        public JsPerson(Container stage)
        {
            graphics = new Graphics();

            // draw a circle, set the lineStyle to zero so the circle doesn't have an outline
            graphics.LineStyle(0);
            graphics.BeginFill(0xFFFF0B);
            graphics.DrawCircle(0, 0, 30);
            graphics.EndFill();

            stage.AddChild(graphics);
        }
    }
}
