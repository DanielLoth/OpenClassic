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
        [InlineData(50, 50, 0)] // Same point
        [InlineData(51, 51, 2)] // 1 tile away diagonally
        [InlineData(49, 49, 2)] // 1 tile away diagonally
        [InlineData(55, 55, 50)] // 5 tiles away diagonally
        [InlineData(45, 45, 50)] // 5 tiles away diagonally
        [InlineData(40, 40, 200)] // 10 tiles away diagonally
        [InlineData(60, 60, 200)] // 10 tiles away diagonally
        public void CalculatesCorrectDistanceSquared(short otherX, short otherY, int expectedDistSquared)
        {
            var subject = new Point(50, 50);
            var other = new Point(otherX, otherY);

            Assert.Equal(Point.DistanceSquared(subject, other), expectedDistSquared);
            Assert.Equal(Point.DistanceSquared(other, subject), expectedDistSquared);
        }

        [Theory]
        [InlineData(50, 50, 50, 50, 0, true)]
        [InlineData(50, 50, 50, 50, 1, true)]
        [InlineData(50, 50, 50, 49, 0, false)]
        [InlineData(50, 50, 49, 49, 0, false)]
        [InlineData(50, 50, 49, 49, 1, true)]
        [InlineData(50, 50, 40, 40, 9, false)]
        [InlineData(50, 50, 40, 40, 10, true)]
        [InlineData(50, 50, 60, 60, 10, true)]
        [InlineData(50, 50, 60, 60, 11, true)]
        [InlineData(50, 50, 60, 60, 9, false)]
        public void DeterminesIfTwoPointsAreWithinRangeOfEachOther(short x, short y,
            short otherX, short otherY, int range, bool expectedWithinRange)
        {
            var thisPoint = new Point(x, y);
            var otherPoint = new Point(otherX, otherY);

            var actuallyWithinRange = Point.WithinRange(thisPoint, otherPoint, range);

            Assert.Equal(expectedWithinRange, actuallyWithinRange);
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
