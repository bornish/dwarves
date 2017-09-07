using System;
using Bridge.Html5;
using Bridge.Pixi;
using Bridge.Utils;
using WebGame.Common;
using WebGame.Common.Connection;
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
        private IRender render;

        public JsGame():base(new JsInput(), new Socket())
        {
            app = Pixi.AutoDetectRenderer(Camera.ScreenWidth, Camera.ScreenHeight);
            Document.Body.AppendChild(app.View);
            personsContainer = new Container();
            mapContainer = new Container();
            rootContainer = new Container();
            render = new JsRender(personsContainer);
            
            
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
            return new JsMap(mapContainer, input);
        }

        internal override Person CreatePerson(long id)
        {
            return new HumanPerson(render, id, input);
        }

        internal override long DateTimeNow()
        {
            return (long)new Bridge.Date().GetTime();
        }

        internal override long GetLong(long i)
        {
            dynamic temp = i;
            long temp2 = temp;
            return temp2;
        }

        private void Animate()
        {
            Window.RequestAnimationFrame(Animate);
            UpdateWorld();
            app.Render(rootContainer);
        }

        
    }
}
