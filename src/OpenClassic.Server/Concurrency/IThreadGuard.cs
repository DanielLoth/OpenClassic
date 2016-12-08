namespace OpenClassic.Server.Concurrency
{
    public interface IThreadGuard
    {
        void CheckCurrentThreadAllowed();
    }
}
