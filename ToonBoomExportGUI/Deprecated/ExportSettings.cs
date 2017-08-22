using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ToonBoomExportGUI
{
	public class ExportSettings
	{
		[XmlIgnore]
		public Uri ExportDirectory;
		[XmlIgnore]
		public Uri FileLocation;

		public String GlobalOptions;

		public String ExportDirectoryString
		{
			get { return  ExportDirectory == null ? null : ExportDirectory.ToString();}
			set { ExportDirectory = value == null ? null : new Uri(value,UriKind.Relative);}
		}

		public List<VectorFile> Files;

		public ExportSettings ()
		{
			Files = new List<VectorFile> ();
			GlobalOptions = "";
		}
	}
}

