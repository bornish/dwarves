
using GameServer.Dwarves.Persons;
using GameServer.Socket;
using System.Collections.Generic;
using WebGame.Common.Connection;
using System;

namespace GameServer.Dwarves.Map
{
    public class MapContainer
    {
        private List<DeferredAction> deferredActions = new List<DeferredAction>();  // события будут выполняться в несколько потоков в нем
        private Dictionary<long, ServerDataPerson> persons = new Dictionary<long, ServerDataPerson>();
        private TileType[,] tiles = new TileType[MapConst.WIDTH, MapConst.HEIGHT];

        internal TileType[,] GetTiles()
        {
            return tiles;
        }

        internal Dictionary<long, ServerDataPerson> getPersons()
        {
            return persons;
        }

        internal void AddAction(DeferredAction action)
        {
            deferredActions.Add(action);
        }

        internal ServerDataPerson FindPerson(long id)
        {
            return persons[id];
        }

        internal List<DeferredAction> getActions()
        {
            return deferredActions;
        }
    }
}
