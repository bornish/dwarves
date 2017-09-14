
using GameServer.Dwarves.Persons;
using System.Collections.Generic;
using WebGame.Common.Connection;
using GameServer.Dwarves.PersonActions;
using System;

namespace GameServer.Dwarves.Map
{
    public class MapContainer
    {
        private List<DeferredAction> deferredActions = new List<DeferredAction>();  // события будут выполняться в несколько потоков в нем
        private Dictionary<long, ServerDataPerson> persons = new Dictionary<long, ServerDataPerson>();
        private TileType[,] tiles;
        private Dictionary<string, Thing> things = new Dictionary<string, Thing>();

        internal TileType[,] GetTiles()
        {
            return tiles;
        }

        internal Dictionary<long, ServerDataPerson> GetPersons()
        {
            return persons;
        }

        public Dictionary<string, Thing> GetThings()
        {
            return things;
        }

        internal void AddAction(DeferredAction action)
        {
            deferredActions.Add(action);
        }

        internal void DigTileFinish(long i, long j)
        {
            if (tiles[i, j] == TileType.Gold)
                AddThing(ThingType.Gold, 10, i, j);
            tiles[i, j] = TileType.Empty;
        }

        private void AddThing(ThingType thingType, long count, long i, long j)
        {
            var thing = new Thing() { type = thingType, count = count, i = i, j = j, guid = Guid.NewGuid().ToString()};
            things[thing.guid] = thing;
        }

        internal ServerDataPerson FindPerson(long id)
        {
            return persons[id];
        }

        internal List<DeferredAction> getActions()
        {
            return deferredActions;
        }

        internal void GenerateTiles()
        {
            var temp = new int[MapConst.WIDTH, MapConst.HEIGHT] 
            { 
                { 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 2, 2, 0, 0, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0 },
                { 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0 },
                { 0, 1, 1, 1, 0, 0, 1, 1, 0, 2, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0 },
                { 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0 },
                { 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0 },
                { 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0 },
                { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0 },
                { 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            };

            tiles = new TileType[MapConst.WIDTH, MapConst.HEIGHT];
            for (var i = 0; i < MapConst.WIDTH; ++i)
                for (var j = 0; j < MapConst.HEIGHT; ++j)
                    tiles[i, j] = (TileType)temp[i, j];
        }
    }
}
