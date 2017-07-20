using System;
using Bridge.Html5;
using Bridge.Pixi;
using WebGame.Common;
using WebGame.Common.Types;
using WebGame.PixiJs.Display;
using WebGame.Common.Display;

namespace WebGame.PixiJs
{
    class JsGame : Game
    {
        

        private IRenderer app;
        private Container rootContainer;
        private Container personsContainer;
        private Container mapContainer;

        public JsGame():base(new JsInput(), new Socket())
        {
            app = Pixi.AutoDetectRenderer(Camera.ScreenWidth, Camera.ScreenHeight);
            Document.Body.AppendChild(app.View);
            personsContainer = new Container();
            mapContainer = new Container();
            rootContainer = new Container();
            
            
            rootContainer.AddChild(mapContainer);
            rootContainer.AddChild(personsContainer);
            Animate();
        }

        public void InitAll()
        {
            Init(new JsCamera(rootContainer));
        }

        internal override Map CreateMap()
        {
            return new JsMap(mapContainer);
        }

        internal override Person CreatePerson()
        {
            return new JsPerson(personsContainer);
        }

        private void Animate()
        {
            Window.RequestAnimationFrame(Animate);

            app.Render(rootContainer);
        }
    }
}
