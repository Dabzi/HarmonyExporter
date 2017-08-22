using System;
using Gtk;
using System.IO;
namespace ToonBoomExportGUI
{
	public partial class ExportResultDialog : Gtk.Dialog
	{
		public ExportResultDialog (string result)
		{
			this.Build ();

			TextTag info = new TextTag ("<INFO>");
			TextTag warn = new TextTag ("<WARN>");
			warn.Background = "#ff9900";
			TextTag err = new TextTag ("<ERROR>");
			err.Background = "#FF0000";

			Output.Buffer.TagTable.Add (info);
			Output.Buffer.TagTable.Add (warn);
			Output.Buffer.TagTable.Add (err);

			TextTag current = info;

			StringReader reader = new StringReader(result);
			string line;
			while ((line = reader.ReadLine ()) != null) {
				if (line.IndexOf ("INFO:", StringComparison.CurrentCulture) != -1) current = info;
				else if (line.IndexOf ("ERROR:", StringComparison.CurrentCulture) != -1) current = err;
				else if (line.IndexOf ("WARNING:", StringComparison.CurrentCulture) != -1) current = warn;
				else Output.Buffer.Text += "\t";
				TextIter end = Output.Buffer.EndIter;
				Output.Buffer.InsertWithTagsByName (ref end, line + "\n", current.Name);
			}
		}
	}
}

