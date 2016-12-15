using OpenClassic.Server.Collections;
using System.Diagnostics;

namespace OpenClassic.Server.Domain
{
    public partial class Player
    {
        private readonly EntityCollection<IPlayer> watchedPlayers = new EntityCollection<IPlayer>();
        private readonly EntityCollection<INpc> watchedNpcs = new EntityCollection<INpc>();

        public EntityCollection<IPlayer> WatchedPlayers => watchedPlayers;
        public EntityCollection<INpc> WatchedNpcs => watchedNpcs;

        public void RevalidateWatchedPlayers()
        {

        }

        public void UpdateWatchedPlayers()
        {

        }

        public void RevalidateWatchedNpcs()
        {
            foreach (var npc in watchedNpcs.Known)
            {
                if (!WithinRange(npc) || !npc.Active)
                {
                    watchedNpcs.Remove(npc);
                }
            }
        }

        public void UpdateWatchedNpcs()
        {

        }

        private bool WithinRange(ILocatable other)
        {
            Debug.Assert(other != null);

            return Point.WithinRange(Location, other.Location, 16);
        }
    }
}
