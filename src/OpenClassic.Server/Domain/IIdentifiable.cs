using System;

namespace OpenClassic.Server.Domain
{
    public interface IIdentifiable : IEquatable<IIdentifiable>
    {
        short Id { get; set; }
    }
}
