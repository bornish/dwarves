using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridge.Pixi;
using WebGame.Common.Display;
using WebGame.Common.Connection;

namespace WebGame.PixiJs.Display
{
    class JsMap : Map
    {
        private Graphics graphics;

        public JsMap(Container mapContainer)
        {
            graphics = new Graphics();

            mapContainer.AddChild(graphics);
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
                    else
                    {
                        graphics.BeginFill(0xFFFFFF);
                        graphics.DrawRect(i * MapConst.TILE_SIZE, j * MapConst.TILE_SIZE, MapConst.TILE_SIZE, MapConst.TILE_SIZE);
                        graphics.EndFill();
                    }
                }
            }

        }
    }
}
