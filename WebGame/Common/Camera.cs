
namespace WebGame.Common
{
    abstract class Camera
    {
        public const int ScreenWidth = 1500;
        public const int ScreenHeight = 800;

        private float offsetX;
        private float offsetY;
        private float x;
        private float y;
        private float scale = 1;

        internal void SetPersonPosition(float x, float y)
        {
            this.x = x;
            this.y = y;
            UpdateOffset(x, y);
            SetCameraOffset(offsetX, offsetY);
        }

        private void UpdateOffset(float x, float y)
        {
            offsetX = x * scale - ScreenWidth / 2 ;
            offsetY = y * scale - ScreenHeight / 2 ;
        }

        internal abstract void SetCameraOffset(float x, float y);

        internal void ScaleUp()
        {
            scale += 0.1f;
            if (scale > 1)
                scale = 1;

            UpdateOffset(x, y);
            SetCameraOffset(offsetX, offsetY);
            SetScale(scale);
        }

        internal void ScaleDown()
        {
            scale -= 0.1f;
            if (scale < 0.5f)
                scale = 0.5f;

            UpdateOffset(x, y);
            SetCameraOffset(offsetX, offsetY);
            SetScale(scale);
        }

        internal abstract void SetScale(float scale);
    }
}
