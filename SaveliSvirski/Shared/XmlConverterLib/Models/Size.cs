using System.Xml.Serialization;

namespace XmlConverterLib.Models
{
    [Serializable]
    public class Size
    {
        [XmlAttribute]
        public string? Description { get; set; }

        [XmlElement("ColorSwatch")]
        public List<ColorSwatch> Colors { get; set; }
    }
}