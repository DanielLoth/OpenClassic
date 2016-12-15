namespace OpenClassic.Server.Domain
{
    public partial class Player
    {
        private const byte DEFAULT_BODY = 2;
        private const byte DEFAULT_HAIR_COLOUR = 2;
        private const byte DEFAULT_HEAD = 1;
        private const byte DEFAULT_SKIN_COLOUR = 0;
        private const byte DEFAULT_TOP_COLOUR = 8;
        private const byte DEFAULT_TROUSER_COLOUR = 14;

        public byte HairColour { get; set; } = DEFAULT_HAIR_COLOUR;
        public byte TopColour { get; set; } = DEFAULT_TOP_COLOUR;
        public byte TrouserColour { get; set; } = DEFAULT_TROUSER_COLOUR;
        public byte SkinColour { get; set; } = DEFAULT_SKIN_COLOUR;

        public int Head { get; set; } = DEFAULT_HEAD;
        public int Body { get; set; } = DEFAULT_BODY;
        public bool Male { get; set; } = true;
        public bool AppearanceUpdateRequired { get; set; } = true;

        public int GetSprite(int pos)
        {
            switch (pos)
            {
                case 0: return Head;
                case 1: return Body;
                case 2: return 3; // What is 3?
                default: return 0;
            }
        }

        public int[] GetSprites()
        {
            return new int[] { Head, Body, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        }
    }
}
