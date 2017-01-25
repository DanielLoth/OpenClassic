using System.Diagnostics;

namespace OpenClassic.Server.Domain
{
    public class Path
    {
        private readonly int _startX;
        private readonly int _startY;
        private readonly byte[] _xWaypoints;
        private readonly byte[] _yWaypoints;

        public int StartX => _startX;
        public int StartY => _startY;

        public int Length => _xWaypoints.Length;

        public Path(int startX, int startY, byte[] xWaypoints, byte[] yWaypoints)
        {
            Debug.Assert(startX >= 0);
            Debug.Assert(startY >= 0);

            Debug.Assert(xWaypoints != null);
            Debug.Assert(yWaypoints != null);

            _startX = startX;
            _startY = startY;
            _xWaypoints = xWaypoints;
            _yWaypoints = yWaypoints;
        }

        public int GetWaypointX(int waypoint)
        {
            return _startX + (waypoint >= Length ? 0 : _xWaypoints[waypoint]);
        }

        public int GetWaypointY(int waypoint)
        {
            return _startY + (waypoint >= Length ? 0 : _yWaypoints[waypoint]);
        }

        public override string ToString()
        {
            return $"Start=({_startX},{_startY}; Len={_xWaypoints.Length}";
        }
    }
}
