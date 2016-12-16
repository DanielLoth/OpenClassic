using Newtonsoft.Json;
using OpenClassic.Server.Util;
using System.Collections.Generic;
using Xunit;

namespace OpenClassic.Server.Tests.Util
{
    public class DataLoaderTests
    {
        [Fact]
        public void Blah()
        {
            //var folderPath = @"C:\Users\daniel\Source\Repos\OpenClassic\src\OpenClassic.Server\GameData\Locations";

            //DataLoader.LoadFolder(folderPath);

            //var xxx = DataLoader.LoadFile(
            //    @"C:\Users\daniel\Source\Repos\OpenClassic\src\OpenClassic.Server\GameData\Definitions\NPCDef.xml.gz");
        }

        [Fact]
        public void Blah2()
        {
            //var npcDefs = DataLoader.GetNpcDefinitions();

            //var json = JsonConvert.SerializeObject(npcDefs, Formatting.Indented);
        }
    }

    #region Scratchpad

    public class Drop
    {
        [JsonConverter(typeof(SingleObjectOrArrayJsonConverter<ItemDropDef>))]
        public List<ItemDropDef> ItemDropDef { get; set; }
    }

    public class ItemDropDef
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public int Weight { get; set; }
    }

    public class Sprite
    {
        [JsonProperty("int")]
        public int[] SpriteInts { get; set; }
    }

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

        public Sprite Sprites { get; set; }

        public int[] Sprites2 { get; set; }

        public int HairColour { get; set; }
        public int TopColour { get; set; }
        public int BottomColour { get; set; }
        public int SkinColour { get; set; }
        public int Camera1 { get; set; }
        public int Camera2 { get; set; }
        public int WalkModel { get; set; }
        public int CombatModel { get; set; }
        public int CombatSprite { get; set; }

        public Drop Drops { get; set; }
        public bool ShouldSerializeDrops()
        {
            return false;
        }
        public bool ShouldSerializeSprites()
        {
            return false;
        }

        [JsonProperty("ItemDrops")]
        public List<ItemDropDef> Drops2 { get; set; }
    }

    #endregion
}
