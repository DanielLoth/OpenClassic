using System;

namespace OpenClassic.Server.Domain
{
    public interface INpc : IEquatable<INpc>, IIndexable, IIdentifiable, ILocatable
    {
        bool Active { get; set; }

        short StartX { get; set; }
        short StartY { get; set; }
        short MinX { get; set; }
        short MaxX { get; set; }
        short MinY { get; set; }
        short MaxY { get; set; }
    }
}
