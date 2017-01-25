using System;

namespace OpenClassic.Server.Domain
{
    public partial class Player
    {
        private Path _path;

        public int CurWaypoint { get; set; } = -1;

        public void UpdatePosition()
        {
            if (!FinishedPath())
            {
                SetNextPosition();
            }
        }

        public void SetPath(Path path)
        {
            CurWaypoint = -1;
            _path = path;
        }


        /////////////////////////

        protected void SetNextPosition()
        {
            int[] newCoords = { -1, -1 };
            if (CurWaypoint == -1)
            {
                if (AtStart())
                {
                    CurWaypoint = 0;
                }
                else
                {
                    newCoords = GetNextCoords(_location.X, _path.StartX, _location.Y, _path.StartY);
                }
            }
            if (CurWaypoint > -1)
            {
                if (AtWaypoint(CurWaypoint))
                {
                    CurWaypoint++;
                }
                if (CurWaypoint < _path.Length)
                {
                    newCoords = GetNextCoords(_location.X, _path.GetWaypointX(CurWaypoint), _location.Y, _path.GetWaypointY(CurWaypoint));
                }
                else
                {
                    ResetPath();
                }
            }
            if (newCoords[0] > -1 && newCoords[1] > -1)
            {
                Location = new Point((short)newCoords[0], (short)newCoords[1]);
            }
        }

        private bool isBlocking(int x, int y, int bit)
        {
            TileValue t = _world.TileValues[x, y];
            return isBlocking(t.MapValue, (byte)bit) || isBlocking(t.ObjectValue, (byte)bit);
        }

        private bool isBlocking(byte val, byte bit)
        {
            if ((val & bit) != 0)
            { // There is a wall in the way
                return true;
            }
            if ((val & 16) != 0)
            { // There is a diagonal wall here: \
                return true;
            }
            if ((val & 32) != 0)
            { // There is a diagonal wall here: /
                return true;
            }
            if ((val & 64) != 0)
            { // This tile is unwalkable
                return true;
            }
            return false;
        }


        protected int[] GetNextCoords(int startX, int destX, int startY, int destY)
        {
            try
            {
                int[] coords = { startX, startY };
                bool myXBlocked = false, myYBlocked = false, newXBlocked = false, newYBlocked = false;
                if (startX > destX)
                {
                    myXBlocked = isBlocking(startX - 1, startY, 8); // Check right tiles left wall
                    coords[0] = startX - 1;
                }
                else if (startX < destX)
                {
                    myXBlocked = isBlocking(startX + 1, startY, 2); // Check left tiles right wall
                    coords[0] = startX + 1;
                }

                if (startY > destY)
                {
                    myYBlocked = isBlocking(startX, startY - 1, 4); // Check top tiles bottom wall
                    coords[1] = startY - 1;
                }
                else if (startY < destY)
                {
                    myYBlocked = isBlocking(startX, startY + 1, 1); // Check bottom tiles top wall
                    coords[1] = startY + 1;
                }

                // If both directions are blocked OR we are going straight and the direction is blocked
                if ((myXBlocked && myYBlocked) || (myXBlocked && startY == destY) || (myYBlocked && startX == destX))
                {
                    return CancelCoords();
                }

                if (coords[0] > startX)
                {
                    newXBlocked = isBlocking(coords[0], coords[1], 2); // Check dest tiles right wall
                }
                else if (coords[0] < startX)
                {
                    newXBlocked = isBlocking(coords[0], coords[1], 8); // Check dest tiles left wall
                }

                if (coords[1] > startY)
                {
                    newYBlocked = isBlocking(coords[0], coords[1], 1); // Check dest tiles top wall
                }
                else if (coords[1] < startY)
                {
                    newYBlocked = isBlocking(coords[0], coords[1], 4); // Check dest tiles bottom wall
                }

                // If both directions are blocked OR we are going straight and the direction is blocked
                if ((newXBlocked && newYBlocked) || (newXBlocked && startY == coords[1]) || (myYBlocked && startX == coords[0]))
                {
                    return CancelCoords();
                }

                // If only one direction is blocked, but it blocks both tiles
                if ((myXBlocked && newXBlocked) || (myYBlocked && newYBlocked))
                {
                    return CancelCoords();
                }

                return coords;
            }
            catch (Exception e)
            {
                return CancelCoords();
            }
        }

        private int[] CancelCoords()
        {
            ResetPath();
            return new int[] { -1, -1 };
        }

        public void ResetPath()
        {
            _path = null;
            CurWaypoint = -1;
        }

        /**
         * Checks if we have reached the end of our path
         */
        public bool FinishedPath()
        {
            if (_path == null)
            {
                return true;
            }
            if (_path.Length > 0)
            {
                return AtWaypoint(_path.Length - 1);
            }
            else
            {
                return AtStart();
            }
        }

        /**
         * Checks if we are at the given waypoint
         */
        protected bool AtWaypoint(int waypoint)
        {
            return _path.GetWaypointX(waypoint) == _location.X && _path.GetWaypointY(waypoint) == _location.Y;
        }

        /**
         * Are we are the start of the path?
         */
        protected bool AtStart()
        {
            return _location.X == _path.StartX && _location.Y == _path.StartY;
        }
    }
}
