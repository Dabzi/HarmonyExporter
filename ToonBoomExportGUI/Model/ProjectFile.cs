using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ToonBoomExportGUI
{
	public class ProjectFile
	{
		public String ProjectName { get; set; }

		/// <summary>
		/// List of XStage projects to track.
		/// </summary>
		public List<XmlUri> XStageProjects { get; set; }


		/// <summary>
		/// Lists containing elements and scenes to export (with settings).
		/// </summary>
		public List<ExportList> ExportList { get; set; }


		public ProjectFile ()
		{
			XStageProjects = new List<XmlUri> ();
			ExportList = new List<ExportList> ();
		}
	}
}

