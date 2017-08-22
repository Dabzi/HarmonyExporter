using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ToonBoomExportGUI
{
	public class ProjectFile
	{
		public String ProjectName { get; set; }

		public List<XmlUri> XStageProjects { get; set; }

		public List<ExportList> ExportList { get; set; }


		public ProjectFile ()
		{
			XStageProjects = new List<XmlUri> ();
			ExportList = new List<ExportList> ();
		}
	}
}

