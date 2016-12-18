using System;

namespace OpenClassic.Server.Domain
{
    public interface IGameObject : IEquatable<IGameObject>, IIndexable, IIdentifiable, ILocatable
    {
        int Type { get; set; }
        sbyte Direction { get; set; }
    }
}
