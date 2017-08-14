using System;
using Bridge.Pixi;
using WebGame.Common.Display;
using Bridge.Pixi.Interaction;
using WebGame.Common;
using WebGame.Common.Connection;

namespace WebGame.PixiJs.Display
{
    class JsPerson : Person
    {
        private const float SHIFT_Y = 10;

        delegate void TestDelegate(InteractionEvent e);

        private Graphics graphics;
        private Input input;

        public override float X { get { return graphics.Position.X; } set { graphics.Position.X = value; } }
        public override float Y { get { return graphics.Position.Y; } set { graphics.Position.Y = value; } }

        public JsPerson(long id, Container stage, Input input) : base (id)
        {
            graphics = new Graphics();

            UpdateGraphics(graphics, Direction.Down);

            graphics.Interactive = true;
            var func = new TestDelegate(Click);
            graphics.On("pointerdown", func);

            stage.AddChild(graphics);

            this.input = input;
        }

        private void DrawBody(Graphics graphics, int width, int height)
        {
            graphics.BeginFill(0xA9A9A9);
            
            graphics.MoveTo(-width / 2, SHIFT_Y);
            graphics.BezierCurveTo(-width / 2, -height * 4 / 3 + SHIFT_Y, width / 2, -height * 4 / 3 + SHIFT_Y, width / 2, SHIFT_Y);
            graphics.LineTo(-width / 2, SHIFT_Y);
            
            graphics.EndFill();
        }

        private void DrawHead(Graphics graphics, int heightPerson, Direction direction)
        {
            int radius = 16;
            graphics.BeginFill(0xF4CD8A);
            graphics.DrawCircle(0, -heightPerson + SHIFT_Y, radius);
            graphics.EndFill();
            graphics.BeginFill(0x000000);

            if (direction == Direction.Up)
                return;

            graphics.MoveTo(- radius / 2, -heightPerson + SHIFT_Y - radius / 3);
            graphics.LineTo(- radius / 2 + 2, -heightPerson + SHIFT_Y - radius / 3);
            graphics.MoveTo(radius / 2, -heightPerson + SHIFT_Y - radius / 3);
            graphics.LineTo(radius / 2 - 2, -heightPerson + SHIFT_Y - radius / 3);

            graphics.MoveTo(-5, -heightPerson + SHIFT_Y + radius / 2);
            graphics.LineTo(+5, -heightPerson + SHIFT_Y + radius / 2);
            graphics.EndFill();

            /*
            graphics.BeginFill(0xFF8C00);
            var points = new Point[3];
            points[0] = new Point(-3 * radius / 4, -heightPerson + SHIFT_Y + radius / 2);
            points[1] = new Point(0, -heightPerson + SHIFT_Y + radius * 1.5f);
            points[2] = new Point(3 * radius / 4, -heightPerson + SHIFT_Y + radius / 2);
            graphics.DrawPolygon(points);
            graphics.EndFill();
            */
        }

        private void DrawArms(Graphics graphics, int widthPerson, int heightPerson)
        {
            graphics.BeginFill(0xF4CD8A);
            graphics.DrawCircle(-widthPerson/2 +15, -heightPerson/2 + SHIFT_Y, 9);
            graphics.DrawCircle(widthPerson / 2 - 15, -heightPerson / 2 + SHIFT_Y, 9);
            graphics.EndFill();
        }

            // TODO delete
            private void Click(InteractionEvent e)
        {
            input.Click(id);
        }

        public override void SetDirection(Direction direction)
        {
            if (oldDirection == direction)
                return;

            oldDirection = direction;
            
            UpdateGraphics(graphics, direction);
        }

        private void UpdateGraphics(Graphics graphics, Direction direction)
        {
            graphics.Clear();
            graphics.LineStyle(2, 0x000000);

            int w = 80;
            int h = 40;
            
            if (direction == Direction.Up)
            {
                DrawHead(graphics, h, direction);
                DrawArms(graphics, w, h);
                DrawBody(graphics, w, h);
            }
            else
            {
                DrawBody(graphics, w, h);
                DrawHead(graphics, h, direction);
                DrawArms(graphics, w, h);
            }
        }

        public override void UpdateAnimation()
        {
            // смотрим какие анимации сейчас активны и устанавливаем положение тела исходя из них
        }
    }
}
