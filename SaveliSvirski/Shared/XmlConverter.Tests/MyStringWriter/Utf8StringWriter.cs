using System.Text;

namespace XmlConverter.Tests.MyStringWriter
{
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}