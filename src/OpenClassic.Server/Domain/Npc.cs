using System;

namespace OpenClassic.Server.Domain
{
    public class Npc : INpc, IEquatable<Npc>
    {
        public short Id { get; set; }

        public short Index { get; set; }

        private Point _location = new Point(327, 550);

        public Point Location
        {
            get { return _location; }
            set { _location = value; }
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
