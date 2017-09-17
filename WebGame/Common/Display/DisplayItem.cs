
using Bridge.Pixi.Interaction;
using System;
using WebGame.Common.Connection;

namespace WebGame.Common.Display
{
    class DisplayItem
    {
        public Item baseItem;
        public bool needRemove;
        private IGraphics graph;
        private IRender render;
        private Input input;

        // TODO delete
        delegate void ClickDelegate(InteractionEvent e);

        public DisplayItem(Input input, IRender render, Item item)
        {
            this.baseItem = item;
            this.render = render;
            this.input = input;
            graph = render.CreateGraphicsOnStage();

            graph.SetPositionX(MapConst.TILE_SIZE * item.i);
            graph.SetPositionY(MapConst.TILE_SIZE * item.j);
            graph.LineStyle(2, 0x000000);
            graph.BeginFill(0xFFFF00);
            graph.DrawCircle(MapConst.TILE_SIZE / 2, MapConst.TILE_SIZE / 2, MapConst.TILE_SIZE / 4);
            graph.EndFill();

            graph.SetInteractive(true);
            var func = new ClickDelegate(Click);
            graph.On("pointerdown", func);
        }

        private void Click(InteractionEvent e)
        {
            input.takeItem(baseItem.guid);
        }

        internal void Remove()
        {
            graph.Clear();
            render.RemoveGraphicsOnStage(graph);
        }
    }
}
