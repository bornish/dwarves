using System;
using WebGame.PixiJs.Display;

namespace WebGame.Common.Display
{
    public interface IRender
    {
        IContainer CreateContainerOnStage();

        IContainer CreateContainerOnContainer(IContainer person);

        IGraphics CreateGraphicsOnContainer(IContainer containerAll);
        IGraphics CreateGraphicsOnStage();
        void RemoveGraphicsOnStage(IGraphics graph);
    }

    public interface IContainer
    {
        void SetPositionY(float value);
        void SetPositionX(float value);
        float GetPositionX();
        float GetPositionY();
        void AddChild(JsContainer container);
        void AddChild(JsGraphics graphics);
        void SetInteractive(bool v);
        void On(string v, Delegate func);
        void SetRotation(float value);
    }

    public interface IGraphics
    {
        void On(string v, Delegate func);
        void SetRotation(float value);
        void SetInteractive(bool v);
        void SetPositionY(float value);
        void SetPositionX(float value);
        float GetPositionX();
        float GetPositionY();
        void LineStyle(float lineWidth = 0, int color = 0, float alpha = 1);
        void Clear();
        void BeginFill(int color);
        void MoveTo(float v1, float v2);
        void BezierCurveTo(float cpX, float cpY, float cpX2, float cpY2, float toX, float toY);
        void LineTo(float v1, float v2);
        void EndFill();
        void DrawCircle(float x, float y, float radius);
        void DrawRoundedRect(float x, float y, float width, float height, float radius);
    }
}
