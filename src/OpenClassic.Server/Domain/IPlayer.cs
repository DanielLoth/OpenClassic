namespace OpenClassic.Server.Domain
{
    public interface IPlayer : IIndexable, ILocatable
    {
        bool IsActive { get; set; }
    }
}
