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
            int size = Marshal.SizeOf(typeof(Point));

            Assert.Equal(4, size);
        }
    }
}
