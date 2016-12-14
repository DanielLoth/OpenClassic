using OpenClassic.Server.Util;
using System;
using System.Diagnostics;

namespace OpenClassic.Server.Domain
{
    public struct Point : IEquatable<Point>, IComparable<Point>
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

        public override int GetHashCode() => HashCodeHelper.GetHashCode(X, Y);

        public override string ToString() => $"({X},{Y})";

        public bool Equals(Point other) => X == other.X && Y == other.Y;

        public int DistanceFromOriginSquared()
        {
            // We calculate distance from origin, instead of distance, so that
            // we can avoid working with floating point numbers. Integer
            // calculations are computationally less expensive.
            var distFromOriginSquared = (X * X) + (Y * Y);

            return distFromOriginSquared;
        }

        public int CompareTo(Point other)
        {
            var thisDistFromOrigin = DistanceFromOriginSquared();
            var otherDistFromOrigin = other.DistanceFromOriginSquared();

            return thisDistFromOrigin - otherDistFromOrigin;
        }
    }
}
