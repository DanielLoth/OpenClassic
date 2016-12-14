using OpenClassic.Server.Domain;
using System.Runtime.InteropServices;
using Xunit;

namespace OpenClassic.Server.Tests.Domain
{
    public class PointTests
    {
        [Fact]
        public void HasExpectedStructSize()
        {
            var size = Marshal.SizeOf(typeof(Point));

            Assert.Equal(4, size);
        }

        [Theory]
        [InlineData(5, 5, 50)]
        [InlineData(10, 10, 200)]
        [InlineData(0, 0, 0)]
        public void CalculatesCorrectDistanceFromOriginSquared(short x, short y, int expectedDist)
        {
            var point = new Point(x, y);

            Assert.Equal(point.DistanceFromOriginSquared(), expectedDist);
        }

        [Theory]
        [InlineData(10, 10, 20, 20, -1)]
        [InlineData(20, 20, 10, 10, 1)]
        [InlineData(10, 10, 10, 10, 0)]
        public void CompareToOrdersByDistanceFromOriginAscending(short invokedX, short invokedY, short otherX, short otherY, int expectedSignedInt)
        {
            var invoked = new Point(invokedX, invokedY);
            var other = new Point(otherX, otherY);

            var result = invoked.CompareTo(other);

            if (expectedSignedInt == 0)
            {
                // The 'invoked' point and the 'other' point are in the same position when ordering.
                Assert.True(result == 0);
            }
            else if (expectedSignedInt > 0)
            {
                // The 'invoked' point follows the 'other' point.
                Assert.True(result > 0);
            }
            else if (expectedSignedInt < 0)
            {
                // The 'invoked' point precedes the 'other' point.
                Assert.True(result < 0);
            }
        }
    }
}
