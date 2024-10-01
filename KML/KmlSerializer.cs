using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ForetrexToolbox
{
  class KmlSerializer<T> : XmlSerializer
  {
    [RequiresUnreferencedCode("Calls System.Xml.Serialization.XmlSerializer.XmlSerializer(Type)")]
    public KmlSerializer()
      : base(typeof(T))
    {
    }

    [RequiresUnreferencedCode("Calls System.Xml.Serialization.XmlSerializer.Deserialize(XmlReader)")]
    public T? Deserialize(string xml)
    {
      using (StringReader sr = new StringReader(xml))
      {
        XmlTextReader tr = new XmlTextReader(sr);
        return (T?)Deserialize(tr);
      }
    }

    [RequiresUnreferencedCode("Calls System.Xml.Serialization.XmlSerializer.Deserialize(XmlReader)")]
    public T? DeserializeStream(Stream s)
    {
      XmlTextReader tr = new XmlTextReader(s);
      return (T?)Deserialize(tr);
    }

    [RequiresUnreferencedCode("Calls System.Xml.Serialization.XmlSerializer.Serialize(XmlWriter, Object)")]
    public void SerializeStream(T value, Stream s)
    {
      UTF8Encoding utf8 = new UTF8Encoding();
      XmlTextWriter xtw = new XmlTextWriter(s, utf8);
      xtw.Formatting = Formatting.Indented;
      xtw.Indentation = 2;
      base.Serialize(xtw, value);
    }
  }

}
