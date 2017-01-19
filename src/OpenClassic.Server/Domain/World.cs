using OpenClassic.Server.Collections;
using OpenClassic.Server.Util;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenClassic.Server.Domain
{
    public class World : IWorld
    {
        public static readonly int WorldHeight = 3776;
        public static readonly int WorldWidth = 944;

        public static readonly int SectorHeight = 48;
        public static readonly int SectorWidth = 48;

        private readonly List<IPlayer> _players = new List<IPlayer>(500);
        private readonly List<INpc> _npcs = new List<INpc>(2000);

        private readonly ISpatialDictionary<IPlayer> _playerSpatialMap;
        private readonly ISpatialDictionary<INpc> _npcSpatialMap;
        private readonly ISpatialDictionary<IGameObject> _objectSpatialMap;

        private readonly Tile[,] _tiles = new Tile[WorldWidth, WorldHeight];

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

            InitialiseWorldTileData();
        }

        private void InitialiseWorldTileData()
        {
            var sectorMap = LandscapeLoader.LoadLandscape();

            for (int lvl = 0; lvl < 4; lvl++)
            {
                var wildX = 2304;
                var wildY = 1776 - (lvl * 944);
                for (var sx = 0; sx < 1000; sx += 48)
                {
                    for (var sy = 0; sy < 1000; sy += 48)
                    {
                        var x = (sx + wildX) / 48;
                        var y = (sy + (lvl * 944) + wildY) / 48;
                        var bigX = sx;
                        var bigY = sy + (944 * lvl);

                        var sectorName = $"h{lvl}x{x}y{y}";

                        List<Tile> sectorTiles;
                        if (!sectorMap.TryGetValue(sectorName, out sectorTiles))
                        {
                            Debug.Fail($"Sector tile data could not be loaded for sectorName {sectorName}");
                        }

                        InitialiseSectorTileData(x, y, lvl, sectorTiles, bigX, bigY);
                    }
                }
            }
        }

        private void InitialiseSectorTileData(int sectorX, int sectorY, int height, List<Tile> sectorTiles, int bigX, int bigY)
        {
            for (var y = 0; y < SectorHeight; y++)
            {
                for (var x = 0; x < SectorWidth; x++)
                {
                    var bx = bigX + x;
                    var by = bigY + y;

                    if (!PointWithinWorld(bx, by))
                    {
                        continue;
                    }

                    if ((_tiles[x,y].GroundOverlay & 0xff) == 250)
                    {
                        _tiles[x, y].GroundOverlay = (byte)2;
                    }

                    // TODO: Finish loading tile data.

                    /** break in shit **/
                    //int groundOverlay = _tiles[x, y].GroundOverlay & 0xFF;
                    //if (groundOverlay > 0 && EntityHandler.getTileDef(groundOverlay - 1).getObjectType() != 0)
                    //{
                    //    world.getTileValue(bx, by).mapValue |= 0x40; // 64
                    //}

                    //int verticalWall = s.getTile(x, y).verticalWall & 0xFF;
                    //if (verticalWall > 0 && EntityHandler.getDoorDef(verticalWall - 1).getUnknown() == 0 && EntityHandler.getDoorDef(verticalWall - 1).getDoorType() != 0)
                    //{
                    //    world.getTileValue(bx, by).mapValue |= 1; // 1
                    //    world.getTileValue(bx, by - 1).mapValue |= 4; // 4
                    //}

                    //int horizontalWall = s.getTile(x, y).horizontalWall & 0xFF;
                    //if (horizontalWall > 0 && EntityHandler.getDoorDef(horizontalWall - 1).getUnknown() == 0 && EntityHandler.getDoorDef(horizontalWall - 1).getDoorType() != 0)
                    //{
                    //    world.getTileValue(bx, by).mapValue |= 2; // 2
                    //    world.getTileValue(bx - 1, by).mapValue |= 8; // 8
                    //}

                    //int diagonalWalls = s.getTile(x, y).diagonalWalls;
                    //if (diagonalWalls > 0 && diagonalWalls < 12000 && EntityHandler.getDoorDef(diagonalWalls - 1).getUnknown() == 0 && EntityHandler.getDoorDef(diagonalWalls - 1).getDoorType() != 0)
                    //{
                    //    world.getTileValue(bx, by).mapValue |= 0x20; // 32
                    //}
                    //if (diagonalWalls > 12000 && diagonalWalls < 24000 && EntityHandler.getDoorDef(diagonalWalls - 12001).getUnknown() == 0 && EntityHandler.getDoorDef(diagonalWalls - 12001).getDoorType() != 0)
                    //{
                    //    world.getTileValue(bx, by).mapValue |= 0x10; // 16
                    //}
                }
            }
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

        public bool PointWithinWorld(int x, int y)
        {
            return x >= 0 && x < WorldWidth && y >= 0 && y < WorldHeight;
        }
    }
}
