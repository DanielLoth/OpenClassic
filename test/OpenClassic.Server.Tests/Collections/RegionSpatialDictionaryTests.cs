using DryIoc;
using OpenClassic.Server.Collections;
using OpenClassic.Server.Domain;
using System.Collections.Generic;
using Xunit;

namespace OpenClassic.Server.Tests.Collections
{
    public class RegionSpatialDictionaryTests
    {
        const int MaxHeight = 3776;
        const int MaxWidth = 944;

        public static Container Container
        {
            get
            {
                var container = new Container();
                container.Register<IWorld, World>();
                container.Register<IPlayer, Player>();
                container.Register<ISpatialDictionary<IPlayer>, NaiveSpatialDictionary<IPlayer>>();
                container.Register<ISpatialDictionary<INpc>, NaiveSpatialDictionary<INpc>>();
                container.Register<ISpatialDictionary<IGameObject>, NaiveSpatialDictionary<IGameObject>>();

                return container;
            }
        }

        [Theory]
        [InlineData(7, 8)]
        [InlineData(8, 8)]
        [InlineData(31, 32)]
        [InlineData(32, 32)]
        [InlineData(33, 64)]
        public void CalculatesCorrectBase2BucketSize(int bucketSize, int expectedBase2BucketSize)
        {
            var map = new RegionSpatialDictionary<IPlayer>(MaxHeight, MaxWidth, bucketSize);

            Assert.Equal(expectedBase2BucketSize, map.BucketSize);
        }

        [Theory]
        [MemberData(nameof(SearchParamGenerator))]
        public void FindsEntityWhenExpected(short widthInTiles, short heightInTiles, short regionSize,
            Point entityLoc, Point targetLoc, int searchDist, bool expectToFind)
        {
            var map = new RegionSpatialDictionary<IPlayer>(heightInTiles, widthInTiles, regionSize);
            var player = Container.Resolve<IPlayer>();

            player.Location = entityLoc;
            map.Add(player);

            var nearbyEntities = map.GetObjectsInProximity(targetLoc, searchDist);
            var found = nearbyEntities.Contains(player);

            Assert.Equal(expectToFind, found);
        }

        public static IEnumerable<object[]> SearchParamGenerator
        {
            get
            {
                const int regionSize = 8;
                const int widthInTiles = regionSize * 20;
                const int heightInTiles = regionSize * 20;

                var entityLoc = new Point(10, 10);
                var targetLoc = new Point(50, 50);

                for (var dist = 0; dist < 10; dist++)
                {
                    var expectToFind = Point.WithinRange(entityLoc, targetLoc, dist);

                    yield return new object[]
                    {
                        widthInTiles, heightInTiles, regionSize,
                        entityLoc, targetLoc, dist, expectToFind
                    };
                }
            }
        }
    }
}
