using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace OpenClassic.Server.Configuration
{
    public class Config : IConfig
    {
        public bool BoolTest { get; set; }
        public int IntTest { get; set; }
        public string StringTest { get; set; }

        public string DataFilePath { get; set; }

        public string ServerProtocol { get; set; }

        public string RsaEncryptionKey { get; set; }

        public string RsaDecryptionKey { get; set; }

        public string RsaModulus { get; set; }

        public void Validate()
        {
            ValidateServerProtocol();
            ValidateRsaKeypair();
        }

        private void ValidateServerProtocol()
        {
            var validServerProtocols = new HashSet<string>
            {
                "RSCD",
                "Vortex204",
                "MoparClassic"
            };

            if (!validServerProtocols.Contains(ServerProtocol, StringComparer.OrdinalIgnoreCase))
            {
                var validValues = string.Join(", ", validServerProtocols);
                var message = $"The 'ServerProtocol' is invalid. Please update Settings.json so that 'ServerProtocol' is one of the following values: {validValues}";

                throw new InvalidConfigException(message);
            }
        }

        private void ValidateRsaKeypair()
        {
            if (string.IsNullOrWhiteSpace(RsaEncryptionKey))
            {
                var msg = "The 'RsaEncryptionKey' contains no value. Please update Settings.json with a valid RSA public key.";
                throw new InvalidConfigException(msg);
            }

            if (string.IsNullOrWhiteSpace(RsaDecryptionKey))
            {
                var msg = "The 'RsaDecryptionKey' contains no value. Please update Settings.json with a valid RSA public key.";
                throw new InvalidConfigException(msg);
            }

            if (string.IsNullOrWhiteSpace(RsaModulus))
            {
                var msg = "The 'RsaModulus' contains no value. Please update Settings.json with a valid RSA public key.";
                throw new InvalidConfigException(msg);
            }

            BigInteger pubKey, privKey, mod;

            try
            {
                pubKey = BigInteger.Parse(RsaEncryptionKey);
                privKey = BigInteger.Parse(RsaDecryptionKey);
                mod = BigInteger.Parse(RsaModulus);
            }
            catch
            {
                throw new InvalidConfigException("The RSA settings are incorrect. Please review 'RsaEncryptionKey', " +
                    "'RsaDecryptionKey', and 'RsaModulus' in Settings.json and ensure they're valid base-10 integer strings.");
            }

            var dataBeforeEncryption = new byte[] { 10, 20, 50, 100 };
            var decryptedBigInt = new BigInteger(dataBeforeEncryption.Reverse().ToArray());
            var encrypted = BigInteger.ModPow(decryptedBigInt, pubKey, mod).ToByteArray().Reverse().ToArray();

            var encryptedBigInt = new BigInteger(encrypted.Reverse().ToArray());
            var decrypted = BigInteger.ModPow(encryptedBigInt, privKey, mod).ToByteArray().Reverse().ToArray();

            if (decrypted.Length != dataBeforeEncryption.Length ||
                !Enumerable.SequenceEqual(dataBeforeEncryption, decrypted))
            {
                throw new InvalidConfigException(@"The RSA public and private keys do not correspond. Please review " +
                    "'RsaEncryptionKey', 'RsaDecryptionKey', and 'RsaModulus' in Settings.json and update these " +
                    "values so that they form a valid keypair.");
            }
        }
    }
}
