using System.Text;
using System.Xml.Serialization;
using XmlConverter.Tests.MyStringWriter;
using XmlConverterLib.Models;

namespace XmlConverter.Tests.ParserTests
{
    public class ParserTestsBaseStructure
    {
        public string GetXmlStringFromTestXmlFile(string xmlPath)
        {
            using var stream = new FileStream(xmlPath, FileMode.Open, FileAccess.ReadWrite);
            var reader = new StreamReader(stream);
            var xmlString = reader.ReadToEnd();

            var stringBuilder = new StringBuilder(xmlString);
            stringBuilder.Replace("\n", "");
            stringBuilder.Replace("\r", "");
            stringBuilder.Replace("\t", "");

            return stringBuilder.ToString();
        }

        public string SerializeCatalogObj(Catalog catalog)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var xmlSerializer = new XmlSerializer(typeof(Catalog));

            using var textWriter = new Utf8StringWriter();
            xmlSerializer.Serialize(textWriter, catalog, ns);

            return textWriter.ToString();
        }
    }
}