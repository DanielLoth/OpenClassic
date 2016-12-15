using OpenClassic.Server.Domain;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenClassic.Server.Collections
{
    public class NaiveSpatialDictionary<T> : ISpatialDictionary<T> where T : ILocatable
    {
        private readonly ISet<T> entities = new HashSet<T>();

        public void Add(T entity)
        {
            Debug.Assert(!ReferenceEquals(entity, null));

            entities.Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            Debug.Assert(entities != null);

            foreach (var entity in entities)
            {
                Add(entity);
            }
        }

        public IEnumerable<T> GetObjectsInProximityLazy(Point point, int distance)
        {
            var distanceSquared = 2 * (distance * distance);

            foreach (var otherEntity in entities)
            {
                var thatLoc = otherEntity.Location;

                var distBetweenPoints = Point.DistanceSquared(point, thatLoc);

                if (distBetweenPoints <= distanceSquared)
                {
                    yield return otherEntity;
                }
            }
        }

        public List<T> GetObjectsInProximity(Point point, int distance) => new List<T>(GetObjectsInProximityLazy(point, distance));
        public List<T> GetObjectsInProximity(T entity, int distance) => new List<T>(GetObjectsInProximityLazy(entity.Location, distance));
        public IEnumerable<T> GetObjectsInProximityLazy(T entity, int distance) => GetObjectsInProximityLazy(entity.Location, distance);

        public void Rehash()
        {
            // Do nothing - this map is naive and just loops through all entities when searched.
        }

        public void Remove(T entity)
        {
            Debug.Assert(!ReferenceEquals(entity, null));

            entities.Remove(entity);
        }

        public void UpdateLocation(T entity, Point oldLocation, Point newLocation)
        {
            // Do nothing - this map is naive and just loops through all entities when searched.
        }
    }
}
