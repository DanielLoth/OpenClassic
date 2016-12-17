namespace OpenClassic.Server.Configuration
{
    public interface IConfig
    {
        bool BoolTest { get; set; }
        int IntTest { get; set; }
        string StringTest { get; set; }

        string DataFilePath { get; set; }

        string ServerProtocol { get; set; }

        string RsaEncryptionKey { get; set; }
        string RsaDecryptionKey { get; set; }
        string RsaModulus { get; set; }
    }
}
