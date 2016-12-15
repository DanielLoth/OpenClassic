using System;

namespace OpenClassic.Server.Domain
{
    public interface INpc : IEquatable<INpc>, IIndexable, IIdentifiable, ILocatable
    {
        bool Active { get; set; }
    }
}
