namespace OpenClassic.Server.Domain
{
    public interface IUpdatable
    {
        void PreUpdate();
        void Update();
        void PostUpdate();
    }
}
