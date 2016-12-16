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
            foreach (var npc in watchedNpcs.KnownReadOnly)
            {
                if (!npc.Active || !WithinRange(npc))
                {
                    watchedNpcs.Remove(npc);
                }
            }
        }

        public void UpdateWatchedNpcs()
        {
            foreach (var npc in _npcSpatialMap.GetObjectsInProximityLazy(_location, 16))
            {
                if (!watchedNpcs.Contains(npc) || (watchedNpcs.Removing(npc) && WithinRange(npc)))
                {
                    watchedNpcs.Add(npc);
                }
            }
        }

        private bool WithinRange(ILocatable other)
        {
            Debug.Assert(other != null);

            return Point.WithinRange(Location, other.Location, 16);
        }
    }
}
