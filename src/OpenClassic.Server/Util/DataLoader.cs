using Newtonsoft.Json;
using OpenClassic.Server.Configuration;
using OpenClassic.Server.Domain.Definition;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml.Linq;

namespace OpenClassic.Server.Util
{
    public static class DataLoader
    {
        private static readonly IConfig Config;
        private static readonly string BasePath;

        static DataLoader()
        {
            var configProvider = new JsonConfigProvider();
            Config = configProvider.GetConfig();

            var dataPath = Config.DataFilePath.Trim();
            var dataPathWithTrailingSlash = dataPath.EndsWith("/", StringComparison.OrdinalIgnoreCase) ||
                dataPath.EndsWith(@"\", StringComparison.OrdinalIgnoreCase) ? dataPath : $"{dataPath}/";

            BasePath = dataPathWithTrailingSlash;
        }

        public static List<ItemDefinition> GetItemDefinitions()
        {
            var filePath = $"{BasePath}/Definitions/ItemDef.json";
            return DeserialiseJsonFile<ItemDefinition>(filePath);
        }

        public static List<ItemLocation> GetItemLocations()
        {
            var filePath = $"{BasePath}/Locations/ItemLoc.json";
            return DeserialiseJsonFile<ItemLocation>(filePath);
        }

        public static List<NpcDefinition> GetNpcDefinitions()
        {
            var filePath = $"{BasePath}/Definitions/NPCDef.json";
            return DeserialiseJsonFile<NpcDefinition>(filePath);
        }

        public static List<NpcLocation> GetNpcLocations()
        {
            var filePath = $"{BasePath}/Locations/NpcLoc.json";
            return DeserialiseJsonFile<NpcLocation>(filePath);
        }

        public static List<GameObjectDefinition> GetObjectDefinitions()
        {
            var filePath = $"{BasePath}/Definitions/GameObjectDef.json";
            return DeserialiseJsonFile<GameObjectDefinition>(filePath);
        }

        public static List<GameObjectLocation> GetObjectLocations()
        {
            var filePath = $"{BasePath}/Locations/GameObjectLoc.json";
            return DeserialiseJsonFile<GameObjectLocation>(filePath);
        }

        private static List<T> DeserialiseJsonFile<T>(string filePath)
        {
            var settings = new JsonSerializerSettings();
            settings.MissingMemberHandling = MissingMemberHandling.Error;

            var fileText = File.ReadAllText(filePath);
            var results = JsonConvert.DeserializeObject<List<T>>(fileText, settings);

            return results;
        }

        private static void LoadFolder(string folderPath)
        {
            var filePaths = Directory.GetFiles(folderPath);

            foreach (var path in filePaths)
            {
                if (!path.EndsWith(".xml.gz", StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }

                var fileNameXmlExtStart = path.IndexOf(".xml.gz", StringComparison.CurrentCultureIgnoreCase);
                var fileName = path.Substring(0, fileNameXmlExtStart) + ".json";

                try
                {
                    var fileJson = LoadFileAsJson(path);

                    File.WriteAllText(fileName, fileJson);
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    throw;
                }
            }
        }

        private static string LoadFileAsJson(string filepath)
        {
            if (string.IsNullOrEmpty(filepath) || !File.Exists(filepath))
            {
                throw new ArgumentException("Invalid file path");
            }

            var xml = GetXDocument(filepath);
            var json = JsonConvert.SerializeXNode(xml, Formatting.Indented);

            return json;
        }

        private static XDocument GetXDocument(string filepath)
        {
            Debug.Assert(filepath != null);

            var gzippedFile = File.ReadAllBytes(filepath);

            using (var stream = new GZipStream(new MemoryStream(gzippedFile), CompressionMode.Decompress))
            {
                var xmlDoc = XDocument.Load(stream);

                return xmlDoc;
            }
        }

        private static string GetGzippedFileAsString(string filepath)
        {
            Debug.Assert(filepath != null);

            var gzippedFile = File.ReadAllBytes(filepath);

            using (var stream = new GZipStream(new MemoryStream(gzippedFile), CompressionMode.Decompress))
            {
                var buffer = new byte[4096];

                using (var ms = new MemoryStream())
                {
                    var bytesRead = 0;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        ms.Write(buffer, 0, bytesRead);
                    }

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }
    }
}
