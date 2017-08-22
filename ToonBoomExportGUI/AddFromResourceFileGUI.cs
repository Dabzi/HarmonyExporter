using System;
using System.Collections.Generic;
using Gtk;

namespace ToonBoomExportGUI
{
	public partial class AddFromResourceFileGUI : Gtk.Dialog
	{
		public delegate void FilesSelectedCallback(List<VectorFilePath> vfp);

		private FilesSelectedCallback callback;
		private ExportSettingsController settings;
		private NodeStore nodestore;
		private String elementTableUrl;

		public AddFromResourceFileGUI (ExportSettingsController settings, FilesSelectedCallback callback)
		{
			this.callback = callback;
			this.settings = settings;
	
			Build ();

			nodestore = new NodeStore (typeof(VectorFilePath));

			CellRendererToggle selectToggle = new CellRendererToggle ();
			selectToggle.Activatable = true;
			selectToggle.Toggled += (object o, ToggledArgs args) =>
				{
					VectorFilePath selected = (VectorFilePath)  nodestore.GetNode(new TreePath(args.Path));
					//Invert value
					selected.Selected = !selected.Selected;
				};

			EntryList.AppendColumn("Add", selectToggle, "active", 0);
			EntryList.AppendColumn("Name", new Gtk.CellRendererText (), "text", 1);
			EntryList.AppendColumn("Path", new Gtk.CellRendererText (), "text", 2);


			EntryList.NodeStore = nodestore;
			
		}
			

		protected void OkClicked (object sender, EventArgs e)
		{
			List<VectorFilePath> result = new List<VectorFilePath> ();

			foreach (ITreeNode tn in nodestore)
			{
				VectorFilePath vfp = (VectorFilePath)tn;
				if (vfp.Selected)
				{
					result.Add (vfp);
				}
			}



			callback (result);
			Destroy ();
		}

		protected void CancelClicked (object sender, EventArgs e)
		{
			Destroy ();
		}

		protected void SelectElementTableClicked (object sender, EventArgs e)
		{
			Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog ("Open xstage file.", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
			fc.SetCurrentFolder(settings.GetSettingsFileLocationUri().AbsolutePath);
			FileFilter ff = new FileFilter ();
			ff.AddPattern ("*.xstage");
			fc.Filter = ff;

			if (fc.Run () == (int)ResponseType.Accept) 
			{

				XStageProject xsp = XStageProject.Load(fc.Filename);
				XStageElements elementTable = xsp.elements;

				elementTableUrl = fc.Filename;

				nodestore.Clear ();
				foreach(XStageElement element in elementTable)
				{

					Uri elementTableUri = new Uri (elementTableUrl);
					Uri elementTableDirectory = new Uri (elementTableUri, ".");

					foreach (XStageDrawing d in element.drawings)
					{
						VectorFilePath vfp = new VectorFilePath();
						vfp.Path = elementTableDirectory.OriginalString + element.rootFolder + "/" + element.elementFolder + "/" + element.elementName + "-" + d.name + ".tvg";
						vfp.Name = element.elementName + "-" + d.name;
						nodestore.AddNode (vfp);
					}

				}

				Console.WriteLine (xsp.GetXmlString ());
			}
			fc.Destroy();
		}

	}
}

