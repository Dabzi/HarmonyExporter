using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
namespace ToonBoomExportGUI
{
	public class XmlUri : IXmlSerializable
	{
		private Uri _Value;

		public XmlUri () { }
		public XmlUri (Uri source) { _Value = source; }

		public static implicit operator Uri (XmlUri o)
		{
			return o == null ? null : o._Value;
		}

		public static implicit operator XmlUri (Uri o)
		{
			return o == null ? null : new XmlUri (o);
		}

		public XmlSchema GetSchema ()
		{
			return null;
		}

		public void ReadXml (XmlReader reader)
		{
			_Value = new Uri (reader.ReadElementContentAsString (),UriKind.RelativeOrAbsolute);
		}

		public void WriteXml (XmlWriter writer)
		{
			writer.WriteValue (_Value.OriginalString);
		}
	}
}

