using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Diagnostics;
using System.Xml.Linq;

namespace OpenClassic.Server.Util
{
    public static class DataLoader
    {
        public static void LoadFolder(string folderPath)
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
                    var xxx = 1;
                }
            }
        }

        public static string LoadFileAsJson(string filepath)
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
