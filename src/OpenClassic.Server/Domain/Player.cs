using System;

namespace OpenClassic.Server.Domain
{
    public class Player : IPlayer
    {
        public short Index { get; set; }

        public bool IsActive { get; set; }

        private Point _location = new Point(329, 552);
        public Point Location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
            }
        }
    }
}
