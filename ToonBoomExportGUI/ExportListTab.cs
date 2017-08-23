using System;
using System.Diagnostics;
using Gtk;
using System.Collections.Generic;
namespace ToonBoomExportGUI
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class ExportListTab : Gtk.Bin
	{
		private ExportList exportList;
		private NodeStore nodestore;
		private ExportListController controller;
		private ProjectController projectController;

		public ExportListTab (ExportList list, ProjectController projectController, ExportListController controller)
		{
			this.Build ();
			this.exportList = list;
			this.controller = controller;
			this.projectController = projectController;

			_initNodeStore ();
			_setupExportDestinationButton ();

			ExportLocationEntry.Text = ((Uri)list.ExportDirectory).OriginalString;

			ExportListNameEntry.Text = list.Name;
			ExportListNameEntry.Changed += (sender, e) => {
				list.Name = ExportListNameEntry.Text;
			};

			SuffixEntry.Text = list.Suffix;
			SuffixEntry.Changed += (sender, e) => {
				list.Suffix = SuffixEntry.Text;
			};

			PrefixEntry.Text = list.Prefix;
			PrefixEntry.Changed += (sender, e) => {
				list.Prefix = PrefixEntry.Text;
			};

			GlobalArgsTxt.Text = list.Options;
			GlobalArgsTxt.Changed += (sender, e) => {
				list.Options = GlobalArgsTxt.Text;
			};

			ResXEntry.Text = ""+list.ResolutionX;
			ResXEntry.Changed += (sender, e) => {
				int parseResult;
				bool parsable = int.TryParse (ResXEntry.Text, out parseResult);
				if (parsable) {
					list.ResolutionX = parseResult;
				}

			};
			ResXEntry.FocusOutEvent += (o, args) => {
				ResXEntry.Text = "" + list.ResolutionX;
			};

			ResYEntry.Text = "" + list.ResolutionY;
			ResYEntry.Changed += (sender, e) => {
				int parseResult;
				bool parsable = int.TryParse (ResYEntry.Text, out parseResult);
				if (parsable) {
					list.ResolutionY = parseResult;
				}

			};
			ResYEntry.FocusOutEvent += (o, args) => {
				ResYEntry.Text = "" + list.ResolutionY;
			};

			ListStore formatModel = new ListStore (typeof (string), typeof (ElementExportSettings.CropSetting));
			foreach (object enumVal in Enum.GetValues (typeof (ExportType))) {
				formatModel.AppendValues (enumVal.ToString (), enumVal);
			}
			FormatCombo.Model = formatModel;
			FormatCombo.Active = (int)list.DefaultExportType;
			FormatCombo.Changed += (sender, e) => {
				list.DefaultExportType = (ExportType)FormatCombo.Active;
			};

			ListStore cropModeModel = new ListStore (typeof (string));
			foreach (object enumVal in Enum.GetValues (typeof (ElementExportSettings.CropSetting))) {
				if ((ElementExportSettings.CropSetting)enumVal == ElementExportSettings.CropSetting.Default) continue;
				cropModeModel.AppendValues (enumVal.ToString ());
			}

			CropModeCombo.Model = cropModeModel;
			CropModeCombo.Changed += (sender, e) => {
				TreeIter selected;
				CropModeCombo.GetActiveIter (out selected);
				string selectedValue = (string)cropModeModel.GetValue (selected, 0);
				list.CropSetting = selectedValue;
			};

			//Set combo to current value
			string initialValue = exportList.CropSetting;
			TreeIter pos;

			//Sets to first entry if next function fails due to invalid value.
			CropModeCombo.Model.GetIterFirst (out pos);
			//Iterates until we find a entry which matches the inital value.
			CropModeCombo.Model.Foreach ((model, path, iter) => {
				if (((string)model.GetValue (iter, 0)).Equals (initialValue)) {
					pos = iter;
					return true;
				} else {
					return false;
				}
			});
			CropModeCombo.SetActiveIter (pos);

			System.Action UpdateAllArtButton = () => {
				LineArtButton.Active = list.ExportLineArt;
				ColorArtButton.Active = list.ExportColorArt;
				AllArtButton.Sensitive = !(LineArtButton.Active && ColorArtButton.Active);
				AllArtButton.Active = (LineArtButton.Active && ColorArtButton.Active);
			};
			UpdateAllArtButton ();

			AllArtButton.Clicked += (sender, e) => {
				if (AllArtButton.Active) {
					LineArtButton.Active = true;
					ColorArtButton.Active = true;
					UpdateAllArtButton ();
				}
			};

			LineArtButton.Clicked += (sender, e) => {
				list.ExportLineArt = LineArtButton.Active;
				UpdateAllArtButton ();
			};

			ColorArtButton.Clicked += (sender, e) => {
				list.ExportColorArt = ColorArtButton.Active;
				UpdateAllArtButton ();
			};


			foreach (ElementExportSettings file in list.Elements) {
				nodestore.AddNode (file);
			}

			controller.TvgAdded += (activeList) => {
				if (activeList == list) {
					nodestore.Clear ();
					foreach (ElementExportSettings file in list.Elements) {
						nodestore.AddNode (file);
					}
				}
			};

			ExportSelectedButton.Clicked += (sender, e) => {
				ITreeNode[] selected = NodeFileList.NodeSelection.SelectedNodes;
				List<ElementExportSettings> tvgs = new List<ElementExportSettings> ();
				foreach (ITreeNode node in selected) {
					tvgs.Add ((ElementExportSettings)node);
				}
				if (tvgs.Count > 0) {
					projectController.Export (list, tvgs);
				}
			};

			DeleteExportListButton.Clicked += (sender, e) => {
				projectController.RemoveExportList (exportList);
			};


			RemoveButton.Clicked += (sender, e) => {
				ITreeNode [] selected = NodeFileList.NodeSelection.SelectedNodes;
				List<ElementExportSettings> tvgs = new List<ElementExportSettings> ();
				foreach (ITreeNode node in selected) {
					tvgs.Add ((ElementExportSettings)node);
				}
				foreach (ElementExportSettings tvg in tvgs) {
					controller.RemoveTvg (tvg, exportList);
				}

				//Refresh node store
				nodestore.Clear ();
				foreach (ElementExportSettings file in list.Elements) {
					nodestore.AddNode (file);
				}

			};
		}



		private void _initNodeStore ()
		{
			nodestore = new NodeStore (typeof (ElementExportSettings));

			CellRendererText optionsRenderer = new CellRendererText ();
			optionsRenderer.Editable = true;
			optionsRenderer.Edited += (object o, EditedArgs args) => {
				ElementExportSettings vf = (ElementExportSettings)(NodeFileList.NodeStore.GetNode (new TreePath (args.Path)));
				vf.Options = args.NewText;

			};

			CellRendererText nameRenderer = new CellRendererText ();
			nameRenderer.Editable = true;
			nameRenderer.Edited += (object o, EditedArgs args) => {
				ElementExportSettings vf = (ElementExportSettings)(NodeFileList.NodeStore.GetNode (new TreePath (args.Path)));
				vf.Name = args.NewText;

			};

			NodeFileList.AppendColumn ("Path", new Gtk.CellRendererText (), "text", 0);

			TreeViewColumn nameCol = NodeFileList.AppendColumn ("Name", nameRenderer, "text", 1);
			nameCol.Resizable = true;

			CellRendererCombo cropRenderer = new CellRendererCombo ();
			ListStore model = new ListStore (typeof(string), typeof (ElementExportSettings.CropSetting));
			foreach (object enumVal in Enum.GetValues (typeof (ElementExportSettings.CropSetting)))
			{
				model.AppendValues (enumVal.ToString (), enumVal);
			}
			cropRenderer.HasEntry = false;
			cropRenderer.Editable = true;
			cropRenderer.Model = model;
			cropRenderer.TextColumn = 0;
			cropRenderer.Edited += (o, args) => {
				ElementExportSettings vf = (ElementExportSettings)(NodeFileList.NodeStore.GetNode (new TreePath (args.Path)));
				vf.CropMode = (ElementExportSettings.CropSetting) Enum.Parse (typeof (ElementExportSettings.CropSetting), args.NewText);
			};
			TreeViewColumn cropCol = NodeFileList.AppendColumn ("Crop Mode", cropRenderer, "text", 4);

			TreeViewColumn optionsCol = NodeFileList.AppendColumn ("Options", optionsRenderer, "text", 2);
			optionsCol.Resizable = true;

			CellRendererPixbuf pixbufRenderer = new CellRendererPixbuf ();

			TreeViewColumn pixCol = NodeFileList.AppendColumn ("Export", pixbufRenderer, "stock-id", 5);

			NodeFileList.RowActivated += (o, args) => {
				if (args.Column == pixCol) {
					ElementExportSettings tvg = ((ElementExportSettings) NodeFileList.NodeStore.GetNode (args.Path));
					ExportResultDialog dialog = new ExportResultDialog (tvg.ExportLog);
					dialog.Run ();
					dialog.Destroy ();
				}
			};
			pixCol.Clicked += (sender, e) => {
				
			};

			NodeFileList.AppendColumn ("", new CellRendererText ());

			NodeFileList.Selection.Mode = SelectionMode.Multiple;

			NodeFileList.NodeStore = nodestore;



		}

		private void _setupExportDestinationButton ()
		{

			SelectExportButton.Clicked += (sender, e) => {
				Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog ("Choose directory to export to", (Gtk.Window)Toplevel, FileChooserAction.SelectFolder, "Cancel", ResponseType.Cancel, "Select Folder", ResponseType.Accept);
				Uri ProjectDirectory = new Uri (projectController.FileLocation, "./");
				fc.SetCurrentFolder (ProjectDirectory.AbsolutePath);

				if (fc.Run () == (int)ResponseType.Accept) {
					Uri selectedDirectory = new Uri (fc.Filename);

					Uri relative = ProjectDirectory.MakeRelativeUri (selectedDirectory);
					exportList.ExportDirectory = relative;
				}
				fc.Destroy ();
				ExportLocationEntry.Text = ((Uri)exportList.ExportDirectory).OriginalString;
			};




		}
	}


}

