using DryIoc;
using OpenClassic.Server.Collections;
using OpenClassic.Server.Domain;
using System.Collections.Generic;
using Xunit;

namespace OpenClassic.Server.Tests.Collections
{
    public class NaiveSpatialDictionaryTests
    {
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
        [InlineData(10, 10, 1, 9)]
        [InlineData(10, 10, 2, 25)]
        [InlineData(100, 100, 16, 1089)]
        public void GenerateTilesInRange(short xOrdinate, short yOrdinate, short range, int expectedTileCount)
        {
            var points = new List<Point>();

            var startX = xOrdinate - range;
            var endX = xOrdinate + range;

            var startY = yOrdinate - range;
            var endY = yOrdinate + range;

            for (var x = startX; x <= endX; x++)
            {
                for (var y = startY; y <= endY; y++)
                {
                    points.Add(new Point((short)x, (short)y));
                }
            }

            Assert.NotEmpty(points);
            Assert.Equal(expectedTileCount, points.Count);
        }

        [Theory]
        [InlineData(56, 56, 6, true)]
        [InlineData(55, 55, 5, true)]
        [InlineData(45, 45, 5, true)]
        [InlineData(40, 40, 10, true)]
        [InlineData(40, 40, 9, false)]
        [InlineData(40, 41, 9, false)]
        [InlineData(41, 41, 9, true)]
        [InlineData(21, 21, 30, true)]
        [InlineData(20, 20, 30, true)]
        [InlineData(19, 20, 30, false)]
        [InlineData(100, 100, 50, true)]
        [InlineData(1000, 1000, 950, true)]
        [InlineData(1000, 1000, 949, false)]
        public void ShouldReturnEntityWithinSearchRange(short x, short y, int dist, bool shouldBeReturned)
        {
            var spatialMap = new NaiveSpatialDictionary<IPlayer>();
            var searchPoint = new Point(50, 50);

            var player = Container.Resolve<IPlayer>();
            player.Location = new Point(x, y);

            spatialMap.Add(player);

            var results = spatialMap.GetObjectsInProximity(searchPoint, dist);

            Assert.Equal(shouldBeReturned, results.Contains(player));
        }

        [Fact]
        public void ShouldNotFindRemovedEntity()
        {
            var spatialMap = new NaiveSpatialDictionary<IPlayer>();
            var searchPoint = new Point(50, 50);
            var player = Container.Resolve<IPlayer>();

            player.Location = new Point(51, 51);

            spatialMap.Add(player);

            // First verify that we can find the player initially.
            Assert.True(spatialMap.GetObjectsInProximity(searchPoint, 40).Contains(player));

            // Now remove the player...
            spatialMap.Remove(player);

            // And verify it isn't returned after removal.
            Assert.False(spatialMap.GetObjectsInProximity(searchPoint, 40).Contains(player));
        }
    }
}
