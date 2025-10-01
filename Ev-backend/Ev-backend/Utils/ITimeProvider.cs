namespace Ev_backend.Utils
{
    public interface ITimeProvider
    {
        DateTime UtcNow { get; }
    }
}
