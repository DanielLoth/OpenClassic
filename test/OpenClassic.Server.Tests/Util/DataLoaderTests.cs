using Newtonsoft.Json;
using OpenClassic.Server.Domain;
using OpenClassic.Server.Util;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace OpenClassic.Server.Tests.Util
{
    public class DataLoaderTests
    {
        [Fact]
        public void LoadsNpcDefinitions()
        {
            var npcDefs = DataLoader.GetNpcDefinitions();

            Assert.NotNull(npcDefs);
            Assert.NotEmpty(npcDefs);
        }

        [Fact]
        public void LoadsNpcLocations()
        {
            var npcLocs = DataLoader.GetNpcLocations();

            Assert.NotNull(npcLocs);
            Assert.NotEmpty(npcLocs);
        }

        [Fact]
        public void LoadsItemDefinitions()
        {
            var itemDefs = DataLoader.GetItemDefinitions();

            Assert.NotNull(itemDefs);
            Assert.NotEmpty(itemDefs);
        }

        [Fact]
        public void LoadsItemLocations()
        {
            var itemLocs = DataLoader.GetItemLocations();

            Assert.NotNull(itemLocs);
            Assert.NotEmpty(itemLocs);
        }

        [Fact]
        public void LoadsGameObjectDefinitions()
        {
            var objectDefs = DataLoader.GetObjectDefinitions();

            Assert.NotNull(objectDefs);
            Assert.NotEmpty(objectDefs);

            var xxx = objectDefs.Select(o => o.Type).Distinct().ToList();
        }

        [Fact]
        public void LoadsGameObjectLocations()
        {
            var objectLocs = DataLoader.GetObjectLocations();

            Assert.NotNull(objectLocs);
            Assert.NotEmpty(objectLocs);

            var directions = objectLocs.Select(o => o.Direction).Distinct().ToList();

            var minPoint = new Point(objectLocs.Min(o => o.X), objectLocs.Min(o => o.Y));
            var maxPoint = new Point(objectLocs.Max(o => o.X), objectLocs.Max(o => o.Y));

            var minDir = objectLocs.Min(x => x.Direction);
            var maxDir = objectLocs.Max(x => x.Direction);

            var minType = objectLocs.Min(x => x.Type);
            var maxType = objectLocs.Max(x => x.Type);
        }

        [Fact]
        public void Blah()
        {
            LandscapeLoader.LoadLandscape();
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
