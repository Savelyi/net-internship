using System.Xml.Serialization;

namespace XmlConverterLib.Models
{
    [Serializable]
    public class Product
    {
        [XmlAttribute]
        public string Description { get; set; }

        [XmlAttribute]
        public string ProductImage { get; set; }

        [XmlElement("CatalogItem")]
        public List<CatalogItem> CatalogItems { get; set; }
    }
}