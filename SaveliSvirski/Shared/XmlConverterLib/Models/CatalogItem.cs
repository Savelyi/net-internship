using System.Xml.Serialization;

namespace XmlConverterLib.Models
{
    [Serializable]
    public class CatalogItem
    {
        [XmlAttribute]
        public string? Gender { get; set; }

        public string? ItemNumber { get; set; }
        public double Price { get; set; }

        [XmlElement("Size")]
        public List<Size> Sizes { get; set; }
    }
}