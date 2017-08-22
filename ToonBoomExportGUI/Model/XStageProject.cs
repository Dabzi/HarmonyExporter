using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;

namespace ToonBoomExportGUI
{
	[XmlRoot("project")]
	public class XStageProject
	{
		[XmlAttribute]
		public String source = " ";
		[XmlAttribute]
		public String version ="1";
		[XmlAttribute]
		public String build = "1.2";

		[XmlArray("elements")]
		[XmlArrayItem("element")]
		public XStageElements elements;

		public XStageProject()
		{
		}

		public String GetXmlString()
		{
			XmlSerializer serialiser = new XmlSerializer (typeof(XStageProject));
			StringWriter sw = new StringWriter ();
			serialiser.Serialize (sw, this);
			String ret = sw.ToString ();
			sw.Close ();
			return ret;

		}

		public static XStageProject Load(String filePath)
		{
			XmlSerializer serialiser = new XmlSerializer (typeof(XStageProject));
			FileStream stream = File.OpenRead (filePath);
			XStageProject tbe = (XStageProject)serialiser.Deserialize (stream);
			stream.Close ();
			return tbe;
		}
	}

	public class XStageElements : List<XStageElement>
	{
		public XStageElements ()
		{
		}
	}
		
	public class XStageElement
	{
		[XmlAttribute]
		public String id;
		[XmlAttribute]
		public String elementName;
		[XmlAttribute]
		public String elementFolder;
		[XmlAttribute]
		public String pixmapFormat;
		[XmlAttribute]
		public String scanType;
		[XmlAttribute]
		public String fieldType;
		[XmlAttribute]
		public String vectorType;
		[XmlAttribute]
		public String rootFolder;

		[XmlArray("drawings")]
		[XmlArrayItem("dwg")]
		public List<XStageDrawing> drawings;

		public XStageElement()
		{
		}
	}
		
	public class XStageDrawing
	{
		[XmlAttribute]
		public String name;
	}


}

