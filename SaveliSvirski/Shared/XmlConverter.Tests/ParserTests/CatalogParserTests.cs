using XmlConverterLib.Parser;
using Xunit;

namespace XmlConverter.Tests.ParserTests
{
    public class CatalogParserTests : ParserTestsBaseStructure
    {
        [Theory]
        [InlineData("TestXml1.xml")]
        [InlineData("TestXml2.xml")]
        [InlineData("TestXml3.xml")]
        public void ParseFromXml_ShouldParseCatalogObj_FromXml(string xmlPath)
        {
            //Arrange
            var path = $"./../../../TestXmlFiles/{xmlPath}";
            var catalogMapper = new CatalogXmlParser();
            var xmlStringResultExpected = GetXmlStringFromTestXmlFile(path);

            //Act
            var result = catalogMapper.ParseFromXml(path);
            var xmlStringResultActual = SerializeCatalogObj(result);

            //Assert
            Assert.Equal(xmlStringResultExpected, xmlStringResultActual);
            Assert.NotNull(result);
        }
    }
}