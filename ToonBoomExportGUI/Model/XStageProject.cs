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
		public List<XStageElement> elements;

		[XmlArray("scenes")]
		[XmlArrayItem("scene")]
		public List<XStageScene> scenes;

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

	public class XStageScene
	{
		[XmlAttribute]
		public String name;
		[XmlAttribute]
		public String id;
		[XmlAttribute]
		public int nbframes;

		[XmlArray("columns")]
		[XmlArrayItem("column")]
		public List<XStageColumn> columns;
	}

	public class XStageColumn
	{
		[XmlAttribute]
		public int id;
		[XmlAttribute]
		public int displayOrder;
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

	public class XStageElementSequence
	{
	}

	public class XStageSceneSequence
	{
	}
		
	public class XStageDrawing
	{
		[XmlAttribute]
		public String name;
	}


}

