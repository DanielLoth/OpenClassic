using OpenClassic.Server.Util;
using System;
using System.Diagnostics;

namespace OpenClassic.Server.Domain
{
    public struct Point : IEquatable<Point>
    {
        public readonly short X;
        public readonly short Y;

        public Point(short x, short y)
        {
            Debug.Assert(x >= 0);
            Debug.Assert(y >= 0);

            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point)) return false;

            var that = (Point)obj;
            return X == that.X && Y == that.Y;
        }

        public override int GetHashCode()
        {
            return HashCodeHelper.GetHashCode(X, Y);
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }

        public bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }
    }
}
