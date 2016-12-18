using OpenClassic.Server.Collections;
using System.Collections.Generic;

namespace OpenClassic.Server.Domain
{
    public interface IWorld
    {
        List<IPlayer> Players { get; }
        List<INpc> Npcs { get; }

        ISpatialDictionary<IPlayer> PlayerSpatialMap { get; }
        ISpatialDictionary<INpc> NpcSpatialMap { get; }
        ISpatialDictionary<IGameObject> ObjectSpatialMap { get; }

        void InitialiseWorld(List<IPlayer> players, List<INpc> npcs);

        IPlayer GetAvailablePlayer();
    }
}
