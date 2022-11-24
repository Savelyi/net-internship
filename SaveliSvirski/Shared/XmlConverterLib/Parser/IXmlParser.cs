namespace XmlConverterLib.Parser
{
    public interface IXmlParser<T>
    {
        T ParseFromXml(string xmlFilePath);
    }
}