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
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Direction { get; set; }
        public int Type { get; set; }
    }
}
