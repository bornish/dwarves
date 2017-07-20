using Bridge.Pixi;
using WebGame.Common;

namespace WebGame.PixiJs
{
    class JsCamera : Camera
    {
        private Container rootContainer;
        public JsCamera(Container container)
        {
            rootContainer = container;
        }

        internal override void SetCameraOffset(float x, float y)
        {
            rootContainer.Position.X = -x;
            rootContainer.Position.Y = -y;
        }

        internal override void SetScale(float scale)
        {
            rootContainer.Scale.X = scale;
            rootContainer.Scale.Y = scale;
        }
    }
}
