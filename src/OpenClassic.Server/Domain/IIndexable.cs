using System;

namespace OpenClassic.Server.Domain
{
    public interface IIndexable : IEquatable<IIndexable>
    {
        short Index { get; set; }
    }
}
