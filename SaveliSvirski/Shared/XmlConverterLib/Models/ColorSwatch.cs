using System.Xml.Serialization;

namespace XmlConverterLib.Models
{
    [Serializable]
    public class ColorSwatch
    {
        [XmlAttribute]
        public string? Image { get; set; }

        [XmlText]
        public string Color { get; set; }
    }
}