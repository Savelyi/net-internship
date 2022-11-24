using System.Xml.Serialization;

namespace XmlConverterLib.Models
{
    [Serializable]
    public class Catalog
    {
        [XmlElement("Product")]
        public List<Product> Products { get; set; }
    }
}