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
    }
}
