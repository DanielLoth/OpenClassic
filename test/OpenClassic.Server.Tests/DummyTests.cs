using Xunit;

namespace OpenClassic.Server.Tests
{
    public class DummyTests
    {
        [Fact]
        public void Blah()
        {
            Assert.Null(null);
        }

        [Theory]
        [InlineData(unchecked ((int)-2147483649))]
        [InlineData(int.MaxValue)]
        [InlineData(2)]
        public void Blah2(int a)
        {
            Assert.Null(null);
        }
    }
}
