using OpenClassic.Server.Domain;
using OpenClassic.Server.Util;
using System.Collections.Generic;
using Xunit;

namespace OpenClassic.Server.Tests.Util
{
    public class LandscapeLoaderTests
    {
        public readonly Dictionary<string, List<Tile>> SectorMap;

        public LandscapeLoaderTests()
        {
            SectorMap = LandscapeLoader.LoadLandscape();
        }

        [Fact]
        public void LoadsExpectedSectorCount()
        {
            var expectedSectorCount = 1764;
            Assert.Equal(expectedSectorCount, SectorMap.Values.Count);
        }

        [Fact]
        public void EachSectorHasExpectedTileCount()
        {
            var expectedTileCount = 48 * 48;
            Assert.All(SectorMap.Values, x => Assert.Equal(expectedTileCount, x.Count));
        }
    }
}
