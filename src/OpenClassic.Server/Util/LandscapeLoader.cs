using OpenClassic.Server.Configuration;
using OpenClassic.Server.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace OpenClassic.Server.Util
{
    public static class LandscapeLoader
    {
        private static readonly IConfig Config;
        private static readonly string BasePath;

        public static readonly int SectorWidth = 48;
        public static readonly int SectorHeight = 48;
        public static readonly int SectorTileCount = SectorWidth * SectorHeight;
        public static readonly int SectorContentLength = SectorTileCount * 10;

        static LandscapeLoader()
        {
            var configProvider = new JsonConfigProvider();
            Config = configProvider.GetConfig();

            var dataPath = Config.DataFilePath.Trim();
            var dataPathWithTrailingSlash = dataPath.EndsWith("/", StringComparison.OrdinalIgnoreCase) ||
                dataPath.EndsWith(@"\", StringComparison.OrdinalIgnoreCase) ? dataPath : $"{dataPath}/";

            BasePath = dataPathWithTrailingSlash;
        }

        public static Dictionary<string, List<Tile>> LoadLandscape()
        {
            var landscapeFilePath = $"{BasePath}/Landscape/Landscape.rscd";
            var sectorMap = new Dictionary<string, List<Tile>>();

            using (var archive = ZipFile.OpenRead(landscapeFilePath))
            {
                // 32 KiB buffer - all unzipped landscape sector files are 23 KiB
                var buffer = new byte[32768];
                var ms = new MemoryStream(buffer);

                foreach (var archiveEntry in archive.Entries)
                {
                    using (var entryStream = archiveEntry.Open())
                    using (var reader = new StreamReader(entryStream))
                    {
                        // Write into the MemoryStream
                        reader.BaseStream.CopyTo(ms);
                        CheckContentLength(ms);

                        // Reset the pointer to the start for reading.
                        ms.Position = 0;

                        var sectorName = archiveEntry.Name;
                        var sectorTiles = LoadTiles(ms);
                        sectorMap[sectorName] = sectorTiles;

                        // Cleanup: Reset the MemoryStream's position pointer
                        // for the next iteration.
                        ms.Position = 0;
                    }
                }
            }

            return sectorMap;
        }

        private static void CheckContentLength(MemoryStream ms)
        {
            // Get length before resetting position pointer.
            var actualSectorContentLength = ms.Position;
            if (actualSectorContentLength < SectorContentLength)
            {
                throw new InvalidDataException("Invalid sector content length. " +
                    $"Expected: {SectorContentLength} - " +
                    $"Actual: {actualSectorContentLength}");
            }
        }

        private static void ProcessSector(MemoryStream ms)
        {
            var tiles = LoadTiles(ms);
        }

        private static List<Tile> LoadTiles(MemoryStream ms)
        {
            var tiles = new List<Tile>();

            for (var i = 0; i < SectorTileCount; i++)
            {
                var tile = new Tile();

                tile.GroundElevation = (byte) ms.ReadByte();
                tile.GroundTexture = (byte) ms.ReadByte();
                tile.GroundOverlay = (byte) ms.ReadByte();
                tile.RoofTexture = (byte) ms.ReadByte();
                tile.HorizontalWall = (byte) ms.ReadByte();
                tile.VerticalWall = (byte) ms.ReadByte();

                tile.DiagonalWalls = ms.ReadByte() << 24 |
                    ms.ReadByte() << 16 | ms.ReadByte() << 8 |
                    ms.ReadByte();

                tiles.Add(tile);
            }

            return tiles;
        }
    }
}
