using System;

namespace OpenClassic.Server.Domain
{
    public interface IPlayer : IEquatable<IPlayer>, IIndexable, ILocatable
    {
        bool IsActive { get; set; }
    }
}
