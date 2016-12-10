using DotNetty.Buffers;
using System.Linq;
using System.Numerics;
using Xunit;

namespace OpenClassic.Server.Tests.Networking
{
    public class RsaDecryptionTests
    {
        [Fact]
        public void EncryptRsaBlock()
        {
            var dataBeforeEncryption = new byte[] { 0, 135, 149, 206, 1, 216, 176, 182, 0, 0, 0, 0, 0, 0, 5, 57, 0, 0, 0, 0, 100, 97, 110, 105, 101, 108, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 10, 100, 97, 110, 105, 101, 108, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 10 };
            var expectedDataAfterEncryption = new byte[] { 25, 224, 87, 217, 170, 173, 206, 249, 83, 111, 67, 199, 213, 60, 146, 29, 210, 242, 185, 176, 51, 104, 71, 61, 208, 167, 73, 234, 18, 161, 110, 5, 11, 176, 245, 138, 56, 189, 141, 207, 251, 182, 241, 186, 247, 133, 198, 204, 53, 201, 110, 234, 117, 12, 33, 163, 187, 214, 5, 95, 240, 150, 167, 158 };

            var pubKeyStr = "1370158896620336158431733257575682136836100155721926632321599369132092701295540721504104229217666225601026879393318399391095704223500673696914052239029335";
            var modStr = "1549611057746979844352781944553705273443228154042066840514290174539588436243191882510185738846985723357723362764835928526260868977814405651690121789896823";

            var pubKey = BigInteger.Parse(pubKeyStr);
            var mod = BigInteger.Parse(modStr);

            var encryptedBigInt = new BigInteger(dataBeforeEncryption.Reverse().ToArray());
            var encrypted = BigInteger.ModPow(encryptedBigInt, pubKey, mod).ToByteArray().Reverse().ToArray();

            Assert.Equal(expectedDataAfterEncryption, encrypted);
        }

        [Fact]
        public void DecryptRsaBlock()
        {
            var dataBeforeEncryption = new byte[] { 0, 135, 149, 206, 1, 216, 176, 182, 0, 0, 0, 0, 0, 0, 5, 57, 0, 0, 0, 0, 100, 97, 110, 105, 101, 108, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 10, 100, 97, 110, 105, 101, 108, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 10 };
            var expectedDataAfterEncryption = new byte[] { 25, 224, 87, 217, 170, 173, 206, 249, 83, 111, 67, 199, 213, 60, 146, 29, 210, 242, 185, 176, 51, 104, 71, 61, 208, 167, 73, 234, 18, 161, 110, 5, 11, 176, 245, 138, 56, 189, 141, 207, 251, 182, 241, 186, 247, 133, 198, 204, 53, 201, 110, 234, 117, 12, 33, 163, 187, 214, 5, 95, 240, 150, 167, 158 };

            var sessionKeys = new int[] { 8885710, 30978230, 0, 1337 };
            var uid = 0;
            var user = "daniel              ";
            var pass = "daniel              ";

            Assert.Equal(62, dataBeforeEncryption.Length);
            Assert.Equal(64, expectedDataAfterEncryption.Length);

            Assert.Equal(0, uid);
            Assert.Equal(20, user.Length);
            Assert.Equal(20, pass.Length);

            var privKeyStr = "730546719878348732291497161314617369560443701473303681965331739205703475535302276087891130348991033265134162275669215460061940182844329219743687403068279";
            var modStr = "1549611057746979844352781944553705273443228154042066840514290174539588436243191882510185738846985723357723362764835928526260868977814405651690121789896823";

            var privKey = BigInteger.Parse(privKeyStr);
            var mod = BigInteger.Parse(modStr);

            var encryptedBigInt = new BigInteger(expectedDataAfterEncryption.Reverse().ToArray());

            var decrypted = BigInteger.ModPow(encryptedBigInt, privKey, mod).ToByteArray().Reverse().ToArray();

            Assert.Equal(dataBeforeEncryption, decrypted);

            var buffer = Unpooled.CopiedBuffer(decrypted);
            var decryptedSessionKeys = new int[4];
            for (var i = 0; i < decryptedSessionKeys.Length; i++)
            {
                decryptedSessionKeys[i] = buffer.ReadInt();
            }

            var decryptedUid = buffer.ReadInt();

            Assert.Equal(sessionKeys, decryptedSessionKeys);
            Assert.Equal(uid, decryptedUid);
        }
    }
}
