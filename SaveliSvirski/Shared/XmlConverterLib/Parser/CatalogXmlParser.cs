using System.Xml.Linq;
using XmlConverterLib.Models;

namespace XmlConverterLib.Parser
{
    public class CatalogXmlParser : IXmlParser<Catalog>
    {
        public Catalog ParseFromXml(string xmlFilePath)
        {
            var xmlDoc = XDocument.Load(xmlFilePath);
            var catalog = ParseProducts(xmlDoc.Element("Catalog").Elements("Product"));

            return catalog;
        }

        private Catalog ParseProducts(IEnumerable<XElement> productsNode)
        {
            var instance = new Catalog();
            var productsList = new List<Product>();
            foreach (var prNode in productsNode)
            {
                var product = new Product();

                product.Description = prNode.Attribute("Description").Value;
                product.ProductImage = prNode.Attribute("ProductImage").Value;

                product.CatalogItems = ParseCatalogItems(prNode.Elements("CatalogItem"));

                productsList.Add(product);
            }

            instance.Products = productsList;

            return instance;
        }

        private List<CatalogItem> ParseCatalogItems(IEnumerable<XElement> catalogItemsNode)
        {
            var items = new List<CatalogItem>();

            foreach (var itemNode in catalogItemsNode)
            {
                var catalogItem = new CatalogItem();
                catalogItem.Gender = itemNode.Attribute("Gender")?.Value;
                catalogItem.ItemNumber = itemNode.Element("ItemNumber")?.Value;
                catalogItem.Price = Convert.ToDouble(itemNode.Element("Price")?.Value);

                catalogItem.Sizes = ParseSizes(itemNode.Elements("Size"));
                items.Add(catalogItem);
            }

            return items;
        }

        private List<Size> ParseSizes(IEnumerable<XElement> sizeNode)
        {
            var sizes = new List<Size>();

            foreach (var node in sizeNode)
            {
                sizes.Add(new Size()
                {
                    Description = node.Attribute("Description")?.Value,
                    Colors = ParseColors(node.Elements("ColorSwatch"))
                });
            }

            return sizes;
        }

        private List<ColorSwatch> ParseColors(IEnumerable<XElement> colorsNode)
        {
            var colorSwatches = new List<ColorSwatch>();

            foreach (var cwNode in colorsNode)
            {
                colorSwatches.Add(new ColorSwatch()
                {
                    Color = cwNode.Value,
                    Image = cwNode.Attribute("Image")?.Value
                });
            }

            return colorSwatches;
        }
    }
}