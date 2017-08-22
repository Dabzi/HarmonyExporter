using System;
using Gtk;

namespace ToonBoomExportGUI
{
	/// <summary>
	/// Used when selecting which files to import
	/// </summary>
	public class VectorFilePath : TreeNode
	{
		[Gtk.TreeNodeValue (Column=0)]
		public bool Selected = false;

		[Gtk.TreeNodeValue (Column=1)]
		public String Name = "unnamed";

		[Gtk.TreeNodeValue (Column=2)]
		public String Path = "";

		public VectorFilePath ()
		{
			
		}
	}
}

