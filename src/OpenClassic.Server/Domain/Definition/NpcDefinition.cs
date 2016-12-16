using System.Collections.Generic;

namespace OpenClassic.Server.Domain.Definition
{
    public class NpcDefinition
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Command { get; set; }

        public int Attack { get; set; }
        public int Strength { get; set; }
        public int Hits { get; set; }
        public int Defense { get; set; }

        public bool Attackable { get; set; }
        public bool Aggressive { get; set; }

        public int RespawnTime { get; set; }

        public int[] Sprites { get; set; }

        public int HairColour { get; set; }
        public int TopColour { get; set; }
        public int BottomColour { get; set; }
        public int SkinColour { get; set; }
        public int Camera1 { get; set; }
        public int Camera2 { get; set; }
        public int WalkModel { get; set; }
        public int CombatModel { get; set; }
        public int CombatSprite { get; set; }

        public List<NpcItemDrop> ItemDrops { get; set; }
    }

    public class NpcItemDrop
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public int Weight { get; set; }
    }
}
