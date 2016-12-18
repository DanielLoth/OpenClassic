namespace OpenClassic.Server.Domain.Definition
{
    public class ItemDefinition
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Command { get; set; }
        public int Sprite { get; set; }
        public int BasePrice { get; set; }
        public bool Stackable { get; set; }
        public bool Wieldable { get; set; }
        public int PictureBitMask { get; set; }
    }

    public class ItemLocation
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Amount { get; set; }
        public int RespawnTime { get; set; }
    }
}
