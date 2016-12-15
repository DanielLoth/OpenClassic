using OpenClassic.Server.Util;
using Xunit;

namespace OpenClassic.Server.Tests.Util
{
    public class UsernameHelperTests
    {
        [Theory]
        [InlineData("Lothy", 23277428)]
        [InlineData("R O F L", 46211196003)]
        [InlineData("Newbsphere", 1839216666962059)]
        public void CalculatesExpectedHashFromUsernameString(string username, long expectedHash)
        {
            var actualHash = UsernameHelper.UsernameToHash(username);

            Assert.Equal(expectedHash, actualHash);
        }

        [Theory]
        [InlineData(23277428, "Lothy")]
        [InlineData(46211196003, "R O F L")]
        [InlineData(1839216666962059, "Newbsphere")]
        public void CalculatesExpectedUsernameStringFromHash(long hash, string expectedUsername)
        {
            var actualUsername = UsernameHelper.HashToUsername(hash);

            Assert.Equal(expectedUsername, actualUsername);
        }
    }
}
