using System;

namespace OpenClassic.Server.Domain
{
    public interface ILocatable : IEquatable<ILocatable>
    {
        bool Active { get; set; }
        Point Location { get; set; }
    }
}
