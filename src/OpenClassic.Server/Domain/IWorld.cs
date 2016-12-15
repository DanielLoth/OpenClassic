using System.Collections.Generic;

namespace OpenClassic.Server.Domain
{
    public interface IWorld
    {
        void InitialiseWorld(List<IPlayer> players, List<INpc> npcs);

        IPlayer GetAvailablePlayer();
    }
}
