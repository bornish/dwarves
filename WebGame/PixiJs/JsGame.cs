using Bridge.Html5;
using Bridge.Pixi;
using WebGame.Common;
using WebGame.PixiJs.Display;
using WebGame.Common.Display;
using System;
using WebGame.Common.Connection;

namespace WebGame.PixiJs
{
    class JsGame : Game
    {
        private IRenderer app;
        private Container rootContainer;
        private Container mapContainer;
        private IRender personRender;
        private IRender thingRender;

        public JsGame():base(new JsInput(), new Socket())
        {
            app = Pixi.AutoDetectRenderer(Camera.ScreenWidth, Camera.ScreenHeight);
            Document.Body.AppendChild(app.View);
            var personsContainer = new Container();
            var thingContainer = new Container();
            mapContainer = new Container();
            rootContainer = new Container();
            personRender = new JsRender(personsContainer);
            thingRender = new JsRender(thingContainer);
            
            rootContainer.AddChild(mapContainer);
            rootContainer.AddChild(thingContainer);
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
            return new HumanPerson(personRender, id, input);
        }

        internal override DisplayItem CreateThing(Item thing)
        {
            // TODO логика слоев должна быть в game. сделать ловлю событий нормально заодно
            return new DisplayItem(input, thingRender, thing);
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
