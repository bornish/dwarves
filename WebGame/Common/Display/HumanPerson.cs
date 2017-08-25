using System;
using Bridge.Pixi.Interaction;
using WebGame.Common.Connection;
using System.Collections.Generic;

namespace WebGame.Common.Display
{
    class HumanPerson : Person
    {
        private Dictionary<AnimationNames, AnimationSource> animations = new Dictionary<AnimationNames, AnimationSource>
        {
            [AnimationNames.Attack] = new AnimationSource(
                (h, t) => h.containerAll.SetPositionX(Spline.Calc(10, - 10, t)),                       (h) => h.containerAll.SetPositionX(0),
                (h, t) => h.containerAll.SetPositionY(Spline.Calc(10 + SHIFT_Y,  - 10 + SHIFT_Y, t)),  (h) => h.containerAll.SetPositionY(0),
                (h, t) => h.containerAll.SetPositionX(Spline.Calc(-10, 10, t)),                        (h) => h.containerAll.SetPositionX(0),
                (h, t) => h.containerAll.SetPositionY(Spline.Calc(-10 + SHIFT_Y, 10 + SHIFT_Y, t)),    (h) => h.containerAll.SetPositionY(0)
            ),
        };

        // TODO delete
        delegate void TestDelegate(InteractionEvent e);
        private Input input;

        private IContainer person;
        public IContainer containerAll;
        private IGraphics body;
        IRender render;

        public HumanPerson(IRender render, long id, Input input) : base(id)
        {
            this.render = render;
            this.input = input;

            // регистрируем в рендере нужные нам объекты
            person = render.CreateContainerOnStage();
            containerAll = render.CreateContainerOnContainer(person);
            body = render.CreateGraphicsOnContainer(containerAll);
             
            person.SetInteractive(true);
            var func = new TestDelegate(Click);
            person.On("pointerdown", func);

            // определяем их параметры
            containerAll.SetPositionY(SHIFT_Y);

            UpdateGraphics(body, Direction.Down);

            
        }

        private void Click(InteractionEvent e)
        {
            input.Click(id);
        }

        public override float X { get { return person.GetPositionX(); } set { person.SetPositionX(value); } }
        public override float Y { get { return person.GetPositionY(); } set { person.SetPositionY(value); } }

        public override void SetDirection(Direction direction)
        {
            if (oldDirection == direction)
                return;

            oldDirection = direction;

            UpdateGraphics(body, direction);
        }

        public override void UpdateAnimation(long currentTime)
        {
            if (currentAnimation != null)
            {
                var t = (float)(currentTime - currentAnimation.timeStart) / currentAnimation.duration;

                if (t < 1)
                {
                    ShowAnimation(currentAnimation.name, t);
                }
                else
                {
                    ClearAnimation(currentAnimation.name);
                    currentAnimation = null;
                }
            }
        }

        private void ClearAnimation(AnimationNames name)
        {
            animations[name].ClearFor(oldDirection)(this);
        }

        private void ShowAnimation(AnimationNames name, float t)
        {
            animations[name].DoFor(oldDirection)(this, t);
        }

        private void UpdateGraphics(IGraphics graphics, Direction direction)
        {
            graphics.Clear();
            graphics.LineStyle(2, 0x000000);

            int w = 80;
            int h = 40;

            if (direction == Direction.Up)
            {
                DrawHead(graphics, h, direction);
                DrawLeftArm(graphics, w, h, direction);
                DrawRightArm(graphics, w, h, direction);
                DrawBody(graphics, w, h);
            }
            else if (direction == Direction.Left)
            {
                DrawRightArm(graphics, w, h, direction);
                DrawBody(graphics, w, h);
                DrawHead(graphics, h, direction);
                DrawLeftArm(graphics, w, h, direction);
                
            }
            else if (direction == Direction.Down)
            {
                DrawBody(graphics, w, h);
                DrawHead(graphics, h, direction);
                DrawLeftArm(graphics, w, h, direction);
                DrawRightArm(graphics, w, h, direction);
            }
            else if (direction == Direction.Right)
            {
                DrawLeftArm(graphics, w, h, direction);
                DrawBody(graphics, w, h);
                DrawHead(graphics, h, direction);
                DrawRightArm(graphics, w, h, direction);
            }
        }

        private void DrawBody(IGraphics graphics, int width, int height)
        {
            graphics.BeginFill(0xA9A9A9);

            graphics.MoveTo(- width / 2, 0);
            graphics.BezierCurveTo(- width / 2, -height * 4 / 3, width / 2, -height * 4 / 3, width / 2, 0);
            graphics.LineTo(- width / 2, 0);

            graphics.EndFill();
        }

        private void DrawHead(IGraphics graphics, int heightPerson, Direction direction)
        {
            int radius = 16;
            var shiftX = 0;
            if (direction == Direction.Left)
                shiftX = -5;
            else if (direction == Direction.Right)
                shiftX = 5;

            graphics.BeginFill(0xF4CD8A);
            graphics.DrawCircle(0, -heightPerson, radius);
            graphics.EndFill();
            graphics.BeginFill(0x000000);

            if (direction == Direction.Up)
                return;

            graphics.MoveTo(-radius / 2 + shiftX, -heightPerson - radius / 3);
            graphics.LineTo(-radius / 2 + 2 + shiftX, -heightPerson - radius / 3);
            graphics.MoveTo(radius / 2 + shiftX, -heightPerson - radius / 3);
            graphics.LineTo(radius / 2 - 2 + shiftX, -heightPerson - radius / 3);

            graphics.MoveTo(-5 + shiftX, -heightPerson + radius / 2);
            graphics.LineTo(+5 + shiftX, -heightPerson + radius / 2);
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

        private void DrawLeftArm(IGraphics graphics, int widthPerson, int heightPerson, Direction direction)
        {
            var r = 9;
            graphics.BeginFill(0xF4CD8A);
            if (direction == Direction.Up)
            {
                graphics.DrawCircle(-widthPerson / 2 + 15, -heightPerson / 2, r);
            }
            else if (direction == Direction.Down)
            {
                graphics.DrawCircle(widthPerson / 2 - 15, -heightPerson / 2, r);
            }
            else if (direction == Direction.Right)
            {
                graphics.DrawCircle(widthPerson / 2 -20, -heightPerson / 2, r);
            }
            else if (direction == Direction.Left)
            {
                graphics.DrawCircle(widthPerson / 2 - 30, -heightPerson / 2, r);
            }
            graphics.EndFill();
        }

        private void DrawRightArm(IGraphics graphics, int widthPerson, int heightPerson, Direction direction)
        {
            graphics.BeginFill(0x49311C);
            var round = 2;
            var r = 9;
            var h = 70;
            var w = 5;
            if (direction == Direction.Up)
            {
                graphics.DrawRoundedRect(widthPerson / 2 - 15 - w / 2, -heightPerson / 2 - h + r, w, h, round);
            }
            else if (direction == Direction.Down)
            {
                graphics.DrawRoundedRect(-widthPerson / 2 + 15 - w / 2, -heightPerson / 2 - h + r, w, h, round);
            }
            else if (direction == Direction.Right)
            {
                graphics.DrawRoundedRect(-widthPerson / 2 + 30 - w / 2, -heightPerson / 2 - h + r, w, h, round);
            }
            else if (direction == Direction.Left)
            {
                graphics.DrawRoundedRect(-widthPerson / 2 + 20 - w / 2, -heightPerson / 2 - h + r, w, h, round);
            }
            graphics.EndFill();

            
            graphics.BeginFill(0xF4CD8A);
            if (direction == Direction.Up)
            {
                graphics.DrawCircle(widthPerson / 2 - 15, -heightPerson / 2, r);
            }
            else if (direction == Direction.Down)
            {
                graphics.DrawCircle(-widthPerson / 2 + 15, -heightPerson / 2, r);
            }
            else if (direction == Direction.Right)
            {
                graphics.DrawCircle(-widthPerson / 2 + 30, -heightPerson / 2, r);
            }
            else if (direction == Direction.Left)
            {
                graphics.DrawCircle(-widthPerson / 2 + 20, -heightPerson / 2, r);
            }
            graphics.EndFill();

            
        }
    }
}
