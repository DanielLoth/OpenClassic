namespace OpenClassic.Server.Configuration
{
    public interface IConfig
    {
        bool BoolTest { get; set; }
        int IntTest { get; set; }
        string StringTest { get; set; }
    }
}
