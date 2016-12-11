using DotNetty.Buffers;
using OpenClassic.Server.Networking;
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
            //var encryptedPayload =
            //    new byte[] { 12, 227, 136, 153, 234, 211, 242, 184, 16, 181, 182, 12, 232, 141, 238, 46, 70, 147, 229, 69, 145, 124, 76, 61, 240, 146, 249, 161, 178, 161, 223, 92, 191, 199, 240, 253, 47, 153, 10, 202, 79, 237, 202, 108, 152, 168, 97, 2, 7, 247, 227, 20, 202, 22, 172, 151, 139, 139, 190, 67, 159, 86, 210, 27 };

            var encryptedPayload =
                new byte[] { 23, 45, 54, 73, 4, 135, 175, 124, 136, 197, 17, 237, 30, 185, 126, 201, 159, 149, 16, 37, 180, 249, 28, 66, 147, 192, 180, 155, 92, 252, 221, 195, 187, 99, 36, 0, 211, 118, 171, 253, 58, 237, 59, 203, 111, 255, 14, 92, 147, 190, 71, 113, 100, 79, 133, 219, 182, 25, 45, 42, 11, 200, 18, 203 };

            var privKeyStr = "730546719878348732291497161314617369560443701473303681965331739205703475535302276087891130348991033265134162275669215460061940182844329219743687403068279";
            var modStr = "1549611057746979844352781944553705273443228154042066840514290174539588436243191882510185738846985723357723362764835928526260868977814405651690121789896823";

            var privKey = BigInteger.Parse(privKeyStr);
            var mod = BigInteger.Parse(modStr);

            var encryptedBigInt = new BigInteger(encryptedPayload.Reverse().ToArray());

            var decrypted = BigInteger.ModPow(encryptedBigInt, privKey, mod).ToByteArray().Reverse().ToArray();
            var buffer = Unpooled.CopiedBuffer(decrypted);

            var decryptedSessionKeys = new int[4];
            for (var i = 0; i < decryptedSessionKeys.Length; i++)
            {
                decryptedSessionKeys[i] = buffer.ReadInt();
            }

            var decryptedUid = buffer.ReadInt();
            var decryptedUser = buffer.ReadString().Trim();
            var decryptedPass = buffer.ReadString().Trim();

            Assert.Equal("dan", decryptedUser);
            Assert.Equal("dan", decryptedPass);
            Assert.Equal(0, decryptedUid);
        }
    }
}
