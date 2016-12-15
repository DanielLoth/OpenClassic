using OpenClassic.Server.Domain;
using System.Collections.Generic;

namespace OpenClassic.Server.Collections
{
    public interface ISpatialDictionary<T> where T : ILocatable
    {
        void Add(T entity);

        void AddRange(IEnumerable<T> entities);

        List<T> GetObjectsInProximity(Point point, int distance);

        IEnumerable<T> GetObjectsInProximityLazy(Point point, int distance);

        List<T> GetObjectsInProximity(T entity, int distance);

        IEnumerable<T> GetObjectsInProximityLazy(T entity, int distance);

        void Rehash();

        void Remove(T entity);

        void UpdateLocation(T entity, Point oldLocation, Point newLocation);
    }
}
