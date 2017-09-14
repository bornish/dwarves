using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridge.Pixi;
using WebGame.Common.Display;
using WebGame.Common.Connection;
using Bridge.Pixi.Interaction;
using WebGame.Common;

namespace WebGame.PixiJs.Display
{
    class JsMap : Map
    {
        // TODO delete
        delegate void ClickDelegate(InteractionEvent e);
        private Input input;
        private Graphics graphics;

        public JsMap(Container mapContainer, Input input)
        {
            graphics = new Graphics();
            this.input = input;

            graphics.Interactive = true;
            var func = new ClickDelegate(Click);
            graphics.On("pointerdown", func);

            mapContainer.AddChild(graphics);
        }

        private void Click(InteractionEvent e)
        {
            var point = e.Data.GetLocalPosition(e.Target);
            input.ClickOnLandPosition(point.X, point.Y);
        }

        public void Update(dynamic tiles, int tilesLenghtX, int tilesLenghtY)
        {
            // TODO перерисовывть каждый кадр не практично, хорошо бы получать от сервера число, если оно изменилось то перерисуем.
            graphics.Clear();
            graphics.LineStyle(0);
            for (var i = 0; i < tilesLenghtX; ++i)
            {
                for (var j = 0; j < tilesLenghtY; ++j)
                {
                    if (tiles[i][j] == TileType.Empty)
                    {
                        graphics.BeginFill(0x00FF00);
                        graphics.DrawRect(i * MapConst.TILE_SIZE, j * MapConst.TILE_SIZE, MapConst.TILE_SIZE, MapConst.TILE_SIZE);
                        graphics.EndFill();
                    }
                    else if (tiles[i][j] == TileType.Stone)
                    {
                        graphics.BeginFill(0x654321);
                        graphics.DrawRect(i * MapConst.TILE_SIZE, j * MapConst.TILE_SIZE, MapConst.TILE_SIZE, MapConst.TILE_SIZE);
                        graphics.EndFill();
                    }
                    else if (tiles[i][j] == TileType.Gold)
                    {
                        graphics.BeginFill(0xFFFF00);
                        graphics.DrawRect(i * MapConst.TILE_SIZE, j * MapConst.TILE_SIZE, MapConst.TILE_SIZE, MapConst.TILE_SIZE);
                        graphics.EndFill();
                    }
                }
            }

        }
    }
}
