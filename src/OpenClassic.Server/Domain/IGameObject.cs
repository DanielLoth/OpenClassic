using System;

namespace OpenClassic.Server.Domain
{
    public interface IGameObject : IEquatable<IGameObject>, IIndexable, IIdentifiable, ILocatable
    {
    }
}
