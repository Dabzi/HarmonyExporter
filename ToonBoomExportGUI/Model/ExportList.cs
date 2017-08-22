using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ToonBoomExportGUI
{
	public class ExportList
	{
		public XmlUri ExportDirectory = new Uri("./output", UriKind.Relative);
		public String Name = "Unnamed";
		public String Options= String.Empty;
		public String Prefix = String.Empty;
		public String Suffix = String.Empty;
		public int ResolutionX = 1920, ResolutionY = 1080;
		public ExportType DefaultExportType = ExportType.PNG4;
		public String CropSetting = "None";
		public bool ExportLineArt = true, ExportColorArt = true;
		public bool ExportAllArt {
			get {
				return ExportLineArt && ExportColorArt;
			}
		}
		public List<TvgFileSetting> Files;

		public ExportList (String name)
		{
			Name = name;
			Files = new List<TvgFileSetting> ();
		}


		public ExportList ()
		{
			Files = new List<TvgFileSetting> ();
		}
	}
}

