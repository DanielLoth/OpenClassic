using OpenClassic.Server.Collections;
using System;
using System.Diagnostics;

namespace OpenClassic.Server.Domain
{
    public partial class Player : IPlayer, IEquatable<Player>
    {
        public static readonly Point DEFAULT_LOCATION = new Point(329, 552);

        // TODO: Remove the player from the spatial map on logout.
        private readonly ISpatialDictionary<IPlayer> _playerSpatialMap;
        private readonly ISpatialDictionary<INpc> _npcSpatialMap;
        private readonly ISpatialDictionary<IGameObject> _objectSpatialMap;

        public short Index { get; set; }

        public bool Active { get; set; }

        private Point _location = Point.OUT_OF_BOUNDS_LOCATION;
        public Point Location
        {
            get { return _location; }
            set {
                var oldLocation = _location;

                // Set the new location.
                _location = value;

                _playerSpatialMap.UpdateLocation(this, oldLocation, value);
            }
        }

        public Player(ISpatialDictionary<IPlayer> playerSpatialMap,
            ISpatialDictionary<INpc> npcSpatialMap,
            ISpatialDictionary<IGameObject> objSpatialMap)
        {
            Debug.Assert(playerSpatialMap != null);
            Debug.Assert(npcSpatialMap != null);
            Debug.Assert(objSpatialMap != null);

            _playerSpatialMap = playerSpatialMap;
            _npcSpatialMap = npcSpatialMap;
            _objectSpatialMap = objSpatialMap;
        }

        public bool Equals(IPlayer other) => other == this;
        public bool Equals(Player other) => other == this;
        public bool Equals(IIndexable other) => other == this;
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
            return $"Player Index={Index}, Location=({x},{y})";
        }
    }
}
