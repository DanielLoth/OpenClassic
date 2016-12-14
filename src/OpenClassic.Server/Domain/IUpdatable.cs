using System;

namespace OpenClassic.Server.Domain
{
    public interface IUpdatable : IEquatable<IUpdatable>
    {
        void PreUpdate();
        void Update();
        void PostUpdate();
    }
}
