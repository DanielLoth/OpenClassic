using OpenClassic.Server.Collections;
using System;
using System.Diagnostics;

namespace OpenClassic.Server.Domain
{
    public class Npc : INpc, IEquatable<Npc>
    {
        private readonly ISpatialDictionary<INpc> _npcSpatialMap;

        public bool Active { get; set; }

        public short Id { get; set; }

        public short Index { get; set; }

        private Point _location = new Point(327, 550);

        public Point Location
        {
            get { return _location; }
            set {
                var oldLocation = _location;

                // Set the new location.
                _location = value;

                _npcSpatialMap.UpdateLocation(this, oldLocation, value);
            }
        }

        public short StartX { get; set; }

        public short StartY { get; set; }

        public short MinX { get; set; }

        public short MaxX { get; set; }

        public short MinY { get; set; }

        public short MaxY { get; set; }

        public Npc(ISpatialDictionary<INpc> npcSpatialMap)
        {
            Debug.Assert(npcSpatialMap != null);

            _npcSpatialMap = npcSpatialMap;
        }

        public bool Equals(INpc other) => other == this;
        public bool Equals(Npc other) => other == this;
        public bool Equals(IIndexable other) => other == this;
        public bool Equals(IIdentifiable other) => other == this;
        public bool Equals(ILocatable other) => other == this;
        public override bool Equals(object obj) => obj == this;

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            var x = _location.X;
            var y = _location.Y;
            return $"Npc - Id={Id}, Index={Index}, Location=({x},{y})";
        }
    }
}
