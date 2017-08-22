using System;
using System.Xml.Serialization;
namespace ToonBoomExportGUI
{
	public class VectorFile : Gtk.TreeNode
	{
		[Gtk.TreeNodeValue (Column=0)]
		public String FilePathString
		{
			get { return  FilePath == null ? null : FilePath.ToString();}
			set { FilePath = value == null ? null : new Uri(value, UriKind.Relative);}
		}
			
		[XmlIgnore]
		public Uri FilePath { get; set; }

		[Gtk.TreeNodeValue (Column=1)]
		public bool ExportMerged { get; set ;}

		[Gtk.TreeNodeValue (Column=2)]
		public bool ExportLineArt { get; set ;} 

		[Gtk.TreeNodeValue (Column=3)]
		public bool ExportColorArt { get; set ; }

		[Gtk.TreeNodeValue (Column = 4)]
		public String Options { get; set; }

		[Gtk.TreeNodeValue (Column = 5)]
		public String Name;

		public VectorFile ()
		{
			ExportMerged = false;
			ExportLineArt = true;
			ExportColorArt = true;
			Options = "";
		}
	}
}

