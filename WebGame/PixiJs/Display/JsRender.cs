
using Bridge.Pixi;
using WebGame.Common.Display;

namespace WebGame.PixiJs.Display
{
    public class JsRender : IRender
    {
        private Container stage;

        public JsRender(Container stage)
        {
            this.stage = stage;
        }

        public IContainer CreateContainerOnContainer(IContainer mainContainer)
        {
            var container = new JsContainer();
            mainContainer.AddChild(container);
            return container;
        }

        public IContainer CreateContainerOnStage()
        {
            var container = new JsContainer();
            stage.AddChild(container.value);
            return container;
        }

        public IGraphics CreateGraphicsOnContainer(IContainer container)
        {
            var graphics = new JsGraphics();
            container.AddChild(graphics);
            return graphics;
        }
    }

    public class JsContainer : IContainer
    {
        public Container value;

        public JsContainer()
        {
            value = new Container();
        }

        public void AddChild(JsContainer container)
        {
            value.AddChild(container.value);
        }

        public void AddChild(JsGraphics graphics)
        {
            value.AddChild(graphics.value);
        }

        public float GetPositionX()
        {
            return value.Position.X;
        }

        public float GetPositionY()
        {
            return value.Position.Y;
        }

        public void SetPositionX(float x)
        {
            value.Position.X = x;
        }

        public void SetPositionY(float y)
        {
            value.Position.Y = y;
        }
    }

    public class JsGraphics : IGraphics
    {
        public Graphics value;

        public JsGraphics()
        {
            value = new Graphics();
        }

        public void BeginFill(int color)
        {
            value.BeginFill(color);
        }

        public void BezierCurveTo(float cpX, float cpY, float cpX2, float cpY2, float toX, float toY)
        {
            value.BezierCurveTo(cpX, cpY, cpX2, cpY2, toX, toY);
        }

        public void Clear()
        {
            value.Clear();
        }

        public void DrawCircle(float x, float y, float radius)
        {
            value.DrawCircle(x, y, radius);
        }

        public void EndFill()
        {
            value.EndFill();
        }

        public void LineStyle(float lineWidth = 0, int color = 0, float alpha = 1)
        {
            value.LineStyle(lineWidth, color, alpha);
        }

        public void LineTo(float x, float y)
        {
            value.LineTo(x, y);
        }

        public void MoveTo(float x, float y)
        {
            value.MoveTo(x, y);
        }

        public void DrawRoundedRect(float x, float y, float width, float height, float radius)
        {
            value.DrawRoundedRect(x, y, width, height, radius);
        }
    }
}
