using System;
using Gtk;
using System.Collections;
namespace ToonBoomExportGUI
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class XStageListTab : Gtk.Bin
	{

		private XStageProject _xstage;
		private NodeStore nodestore;
		private ExportListController exportListController;

		public XStageListTab (XStageProject xstage, Uri fileUri, ExportListController exportListController)
		{
			this.Build ();
			_xstage = xstage;
			this.exportListController = exportListController;


			nodestore = new NodeStore (typeof (TvgFileSetting));

			FileTree.RulesHint = true;
			FileTree.Selection.Mode = SelectionMode.Multiple;
			FileTree.AppendColumn ("Name", new Gtk.CellRendererText (), "text", 1);
			FileTree.AppendColumn ("Path", new Gtk.CellRendererText (), "text", 0);

			FileTree.NodeStore = nodestore;
			AddButton.Clicked += (sender, e) => {
				NodeSelection selection = FileTree.NodeSelection;
				foreach (ITreeNode node in selection.SelectedNodes) {
					//Add selected VectorFilePath to the active export list
					TvgFileSetting current = (TvgFileSetting)node;
					TvgFileSetting tvg = new TvgFileSetting ();
					tvg.Name = current.Name;
					tvg.FilePath = current.FilePath;
					exportListController.AddTvg (tvg);
				}
			};



			foreach (XStageElement element in xstage.elements) {

				Uri xstageDirectory = new Uri (fileUri, "./");

				foreach (XStageDrawing d in element.drawings) {
					TvgFileSetting vfp = new TvgFileSetting ();
					vfp.FilePath = new Uri(xstageDirectory.LocalPath + element.rootFolder + "/" + element.elementFolder + "/" + element.elementName + "-" + d.name + ".tvg");
					vfp.Name = element.elementName + "-" + d.name;
					nodestore.AddNode (vfp);
				}
			}
		}
	}
}

