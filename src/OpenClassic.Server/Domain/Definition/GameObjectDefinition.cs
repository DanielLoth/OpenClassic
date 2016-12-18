namespace OpenClassic.Server.Domain.Definition
{
    public class GameObjectDefinition
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Command1 { get; set; }
        public string Command2 { get; set; }
        public int Type { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int GroundItemVar { get; set; }
        public string ObjectModel { get; set; }
    }

    public class GameObjectLocation
    {
        public short Id { get; set; }
        public short X { get; set; }
        public short Y { get; set; }
        public sbyte Direction { get; set; }
        public int Type { get; set; }
    }
}
