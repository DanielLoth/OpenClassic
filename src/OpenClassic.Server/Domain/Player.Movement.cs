namespace OpenClassic.Server.Domain
{
    public partial class Player
    {
        private readonly byte[] xOffsets = new byte[32];
        private readonly byte[] yOffsets = new byte[32];

        public byte[] XOffsets => xOffsets;
        public byte[] YOffsets => yOffsets;

        public short StartX { get; set; }
        public short StartY { get; set; }
        public int StepCount { get; set; }

        public void UpdatePosition()
        {
            var newX = 300 + ((_location.X + 1) % 50);
            var newY = 500 + ((_location.Y + 1) % 50);

            _location = new Point((short)newX, (short)newY);
        }
    }
}
