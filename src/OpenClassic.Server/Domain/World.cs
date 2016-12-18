using OpenClassic.Server.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenClassic.Server.Domain
{
    public class World : IWorld
    {
        private readonly List<IPlayer> _players = new List<IPlayer>(500);
        private readonly List<INpc> _npcs = new List<INpc>(2000);

        private readonly ISpatialDictionary<IPlayer> _playerSpatialMap;
        private readonly ISpatialDictionary<INpc> _npcSpatialMap;
        private readonly ISpatialDictionary<IGameObject> _objectSpatialMap;

        public ISpatialDictionary<IPlayer> PlayerSpatialMap => _playerSpatialMap;
        public ISpatialDictionary<INpc> NpcSpatialMap => _npcSpatialMap;
        public ISpatialDictionary<IGameObject> ObjectSpatialMap => _objectSpatialMap;

        public World(ISpatialDictionary<IPlayer> playerSpatialMap,
            ISpatialDictionary<INpc> npcSpatialMap,
            ISpatialDictionary<IGameObject> objSpatialMap)
        {
            Debug.Assert(playerSpatialMap != null);
            Debug.Assert(npcSpatialMap != null);
            Debug.Assert(objSpatialMap != null);

            _playerSpatialMap = playerSpatialMap;
            _npcSpatialMap = npcSpatialMap;
            _objectSpatialMap = objSpatialMap;
        }

        public void InitialiseWorld(List<IPlayer> players, List<INpc> npcs)
        {
            Debug.Assert(players != null);
            Debug.Assert(npcs != null);

            _players.AddRange(players);
            _npcs.AddRange(npcs);
        }

        public List<IPlayer> Players => _players;

        public List<INpc> Npcs => _npcs;

        public IPlayer GetAvailablePlayer()
        {
            for (var i = 0; i < _players.Count; i++)
            {
                var player = _players[i];
                if (!player.Active)
                {
                    player.Active = true;
                    return player;
                }
            }

            return null;
        }
    }
}
