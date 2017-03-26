using OpenClassic.Server.Extensions;
using System;
using System.Linq;
using Xunit;

namespace OpenClassic.Server.Tests.Extensions
{
    public class ArraySegmentExtensionTests
    {
        [Fact]
        public void MovesAllBytesAfterGivenOffset()
        {
            var buffer = new byte[30];
            var segment = new ArraySegment<byte>(buffer, 10, 10);

            for (byte i = 0; i < buffer.Length; i++)
            {
                buffer[i] = i;
            }

            // Should have values 10 through 19.
            var before = segment.ToList();
            Assert.Equal(10, before.Count);
            for (int i = 0; i < before.Count; i++)
            {
                var expected = (byte)(i + 10);
                Assert.Equal(expected, before[i]);
            }

            segment.MoveDataToStart(5);

            var after = segment.ToList();
            Assert.Equal(10, after.Count);

            // Should have values 15, 16, 17, 18, 19, 15, 16, 17, 18, 19
            Assert.Equal(15, after[0]);
            Assert.Equal(16, after[1]);
            Assert.Equal(17, after[2]);
            Assert.Equal(18, after[3]);
            Assert.Equal(19, after[4]);
            Assert.Equal(15, after[5]);
            Assert.Equal(16, after[6]);
            Assert.Equal(17, after[7]);
            Assert.Equal(18, after[8]);
            Assert.Equal(19, after[9]);
        }

        [Fact]
        public void MovesSpecifiedNumberOfBytesAfterGivenOffset()
        {
            var buffer = new byte[30];
            var segment = new ArraySegment<byte>(buffer, 10, 10);

            for (byte i = 0; i < buffer.Length; i++)
            {
                buffer[i] = i;
            }

            // Should have values 10 through 19.
            var before = segment.ToList();
            Assert.Equal(10, before.Count);
            for (int i = 0; i < before.Count; i++)
            {
                var expected = (byte)(i + 10);
                Assert.Equal(expected, before[i]);
            }

            segment.MoveDataToStart(5, 2);

            var after = segment.ToList();
            Assert.Equal(10, after.Count);

            // Should have values 15, 16, 12, 13, 14, 15, 16, 17, 18, 19
            Assert.Equal(15, after[0]);
            Assert.Equal(16, after[1]);
            Assert.Equal(12, after[2]);
            Assert.Equal(13, after[3]);
            Assert.Equal(14, after[4]);
            Assert.Equal(15, after[5]);
            Assert.Equal(16, after[6]);
            Assert.Equal(17, after[7]);
            Assert.Equal(18, after[8]);
            Assert.Equal(19, after[9]);
        }
    }
}
