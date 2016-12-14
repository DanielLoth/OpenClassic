using System;

namespace OpenClassic.Server.Domain
{
    public interface ILocatable : IEquatable<ILocatable>
    {
        Point Location { get; set; }
    }
}
