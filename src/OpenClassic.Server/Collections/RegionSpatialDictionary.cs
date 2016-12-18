using OpenClassic.Server.Domain;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenClassic.Server.Collections
{
    public class RegionSpatialDictionary<T> : ISpatialDictionary<T> where T : ILocatable
    {
        private readonly int _bucketSize;
        private readonly int _heightInTiles;
        private readonly int _widthInTiles;

        private readonly int _heightInRegions;
        private readonly int _widthInRegions;

        private readonly IBucket<T>[,] _bucketMap;

        public int BucketSize => _bucketSize;

        public RegionSpatialDictionary(int heightInTiles, int widthInTiles, int bucketSize)
        {
            Debug.Assert(bucketSize >= 0);
            Debug.Assert(heightInTiles > 0);
            Debug.Assert(widthInTiles > 0);

            var bucketSizeBase2 = 1;
            while (bucketSizeBase2 < bucketSize)
            {
                bucketSizeBase2 <<= 1;
            }

            var heightInRegions = (heightInTiles / bucketSizeBase2) + 1;
            var widthInRegions = (widthInTiles / bucketSizeBase2) + 1;

            var map = new Bucket<T>[widthInRegions, heightInRegions];
            for (var regionX = 0; regionX < widthInRegions; regionX++)
            {
                for (var regionY = 0; regionY < heightInRegions; regionY++)
                {
                    var minPoint = new Point((short)regionX, (short)regionY);
                    var bucket = new Bucket<T>(minPoint, bucketSizeBase2);

                    map[regionX, regionY] = bucket;
                }
            }

            _bucketSize = bucketSizeBase2;
            _heightInTiles = heightInTiles;
            _widthInTiles = widthInTiles;

            _heightInRegions = heightInRegions;
            _widthInRegions = widthInRegions;

            _bucketMap = map;
        }

        private IBucket<T> GetBucket(T entity)
        {
            return GetBucket(entity.Location);
        }

        private IBucket<T> GetBucket(Point loc)
        {
            var regionX = loc.X / _bucketSize;
            var regionY = loc.Y / _bucketSize;

            return _bucketMap[regionX, regionY];
        }

        public void Add(T entity)
        {
            Debug.Assert(!ReferenceEquals(entity, null));

            var bucket = GetBucket(entity);
            bucket.Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            Debug.Assert(entities != null);

            foreach (var entity in entities)
            {
                Add(entity);
            }
        }

        public List<T> GetObjectsInProximity(T entity, int distance) => new List<T>(GetObjectsInProximityLazy(entity.Location, distance));

        public List<T> GetObjectsInProximity(Point point, int distance) => new List<T>(GetObjectsInProximityLazy(point, distance));

        public IEnumerable<T> GetObjectsInProximityLazy(T entity, int distance) => GetObjectsInProximityLazy(entity.Location, distance);

        public IEnumerable<T> GetObjectsInProximityLazy(Point point, int distance)
        {
            Debug.Assert(point.X >= 0);
            Debug.Assert(point.X <= _widthInTiles);

            Debug.Assert(point.Y >= 0);
            Debug.Assert(point.Y <= _heightInTiles);

            var regionX = point.X / _bucketSize;
            var regionY = point.Y / _bucketSize;
            var surroundingBucketHops = (distance / _bucketSize) + 1;

            var minRegionX = regionX - surroundingBucketHops;
            minRegionX = minRegionX >= 0 ? minRegionX : 0;

            var minRegionY = regionY - surroundingBucketHops;
            minRegionY = minRegionY >= 0 ? minRegionY : 0;

            var maxRegionX = regionX + surroundingBucketHops;
            maxRegionX = maxRegionX >= _widthInRegions ? _widthInRegions : maxRegionX;

            var maxRegionY = regionY + surroundingBucketHops;
            maxRegionY = maxRegionY >= _heightInRegions ? _heightInRegions : maxRegionY;

            for (var x = minRegionX; x < maxRegionX; x++)
            {
                for (var y = minRegionY; y < maxRegionY; y++)
                {
                    var bucket = _bucketMap[x, y];
                    foreach (var entity in bucket.Entities)
                    {
                        if (Point.WithinRange(point, entity.Location, distance))
                        {
                            yield return entity;
                        }
                    }
                }
            }
        }

        public void Rehash()
        {
            // Do nothing.
        }

        public void Remove(T entity)
        {
            Debug.Assert(!ReferenceEquals(entity, null));

            var bucket = GetBucket(entity);
            bucket.Remove(entity);
        }

        public void UpdateLocation(T entity, Point oldLocation, Point newLocation)
        {
            Debug.Assert(!ReferenceEquals(entity, null));

            var oldBucket = GetBucket(oldLocation);
            oldBucket.Remove(entity);

            var newBucket = GetBucket(newLocation);
            newBucket.Add(entity);
        }

        #region Spatial bucket

        interface IBucket<BType> where BType : ILocatable
        {
            ISet<BType> Entities { get; }

            void Add(BType entity);
            void Remove(BType entity);
        }

        class Bucket<BType> : IEnumerable<BType>, IBucket<BType> where BType : ILocatable
        {
            private readonly ISet<BType> entities = new HashSet<BType>();

            private readonly Point minPointInclusive;
            private readonly Point maxPointExclusive;

            public ISet<BType> Entities => entities;

            public Bucket(Point minPoint, int length)
            {
                Debug.Assert(minPoint.X >= 0);
                Debug.Assert(minPoint.Y >= 0);

                minPointInclusive = minPoint;

                var maxX = (short)(minPoint.X + length);
                var maxY = (short)(minPoint.Y + length);
                maxPointExclusive = new Point(maxX, maxY);
            }

            public void Add(BType entity)
            {
                Debug.Assert(!ReferenceEquals(entity, null));

                entities.Add(entity);
            }

            public void Remove(BType entity)
            {
                Debug.Assert(!ReferenceEquals(entity, null));

                entities.Remove(entity);
            }

            public IEnumerator<BType> GetEnumerator()
            {
                foreach (var entity in entities)
                {
                    yield return entity;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        #endregion
    }
}
