using OpenClassic.Server.Collections;
using System.Diagnostics;

namespace OpenClassic.Server.Domain
{
    public partial class Player
    {
        private readonly EntityCollection<IPlayer> watchedPlayers = new EntityCollection<IPlayer>();
        private readonly EntityCollection<INpc> watchedNpcs = new EntityCollection<INpc>();
        private readonly EntityCollection<IGameObject> watchedObjects = new EntityCollection<IGameObject>();

        public EntityCollection<IPlayer> WatchedPlayers => watchedPlayers;
        public EntityCollection<INpc> WatchedNpcs => watchedNpcs;
        public EntityCollection<IGameObject> WatchedObjects => watchedObjects;

        public void RevalidateWatchedPlayers()
        {

        }

        public void RevalidateWatchedNpcs()
        {
            foreach (var npc in watchedNpcs.KnownReadOnly)
            {
                if (!npc.Active || !WithinRange(npc, 16))
                {
                    watchedNpcs.Remove(npc);
                }
            }
        }

        public void RevalidateWatchedObjects()
        {
            foreach (var obj in watchedObjects.KnownReadOnly)
            {
                if (!obj.Active || !WithinRange(obj, 21))
                {
                    watchedObjects.Remove(obj);
                }
            }
        }

        public void UpdateWatchedPlayers()
        {

        }

        public void UpdateWatchedNpcs()
        {
            foreach (var npc in _npcSpatialMap.GetObjectsInProximityLazy(_location, 16))
            {
                if (!watchedNpcs.Contains(npc) || watchedNpcs.Removing(npc))
                {
                    watchedNpcs.Add(npc);
                }
            }
        }

        public void UpdateWatchedObjects()
        {
            foreach (var obj in _objectSpatialMap.GetObjectsInProximityLazy(_location, 21))
            {
                if (obj.Active && !watchedObjects.Contains(obj))
                {
                    watchedObjects.Add(obj);
                }
            }
        }

        private bool WithinRange(ILocatable other, int range)
        {
            Debug.Assert(other != null);
            Debug.Assert(range >= 0);

            return Point.WithinRange(Location, other.Location, range);
        }
    }
}
