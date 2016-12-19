using OpenClassic.Server.Collections;
using System.Diagnostics;

namespace OpenClassic.Server.Domain
{
    public class GameObject : IGameObject
    {
        private readonly ISpatialDictionary<IGameObject> _gameObjectSpatialMap;

        public bool Active { get; set; } = true;

        public short Id { get; set; }

        public short Index { get; set; }

        public int Type { get; set; }

        public sbyte Direction { get; set; }

        private Point _location;
        public Point Location
        {
            get { return _location; }
            set
            {
                var oldLocation = _location;

                // Set the new location.
                _location = value;

                _gameObjectSpatialMap.UpdateLocation(this, oldLocation, value);
            }
        }

        public GameObject(ISpatialDictionary<IGameObject> objSpatialMap)
        {
            Debug.Assert(objSpatialMap != null);

            _gameObjectSpatialMap = objSpatialMap;
        }

        public bool Equals(IIdentifiable other) => other == this;
        public bool Equals(ILocatable other) => other == this;
        public bool Equals(IIndexable other) => other == this;
        public bool Equals(IGameObject other) => other == this;
        public override bool Equals(object obj) => obj == this;

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"GameObject - Id={Id}, Index={Index}, Location={Location}, Direction={Direction}, Type={Type}";
        }
    }
}
