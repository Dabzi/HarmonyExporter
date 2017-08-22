using System;
using Gtk;
using ToonBoomExportGUI;
using System.Collections.Generic;

public partial class MainWindow: Gtk.Window
{

	private ExportSettingsController exportSettingsController;
	private ExporterController exporterController;
	private ConfigController configController;

	private EventHandler MergeActivateAction = (object sender, EventArgs e) =>
	{
	};
	private EventHandler LineArtActivateAction = (object sender, EventArgs e) =>
	{
	};
	private EventHandler ColorArtActivateAction = (object sender, EventArgs e) =>
	{
	};

	private AddFromResourceFileGUI resourceGui;


	private VectorFile selectedFile;

	private static Gtk.TargetEntry[] target_table =
		new TargetEntry[] {
			new TargetEntry ("text/uri-list", 0, 0)
		};

	public MainWindow (ExportSettingsController exportSettingsController, ExporterController exporterController, ConfigController configController) : base (Gtk.WindowType.Toplevel)
	{
		this.exporterController = exporterController;
		this.exportSettingsController = exportSettingsController;
		this.configController = configController;

		Build ();

		UpdateUI ();

		_initNodeStore ();
		_initDragAndDrop ();
		_initToolsColumn ();

		GlobalOptionsEntry.Buffer.Changed+= (sender, e) => exportSettingsController.SetGlobalOptions(GlobalOptionsEntry.Buffer.Text);

		//Autoscroll console
		ConsoleText.SizeAllocated += (o, args) =>
		{
				ConsoleScrollbox.Vadjustment.Value = ConsoleScrollbox.Vadjustment.Upper - ConsoleScrollbox.Vadjustment.PageSize;
		};
		
		ExportMenuButton.Activated += (sender, e) => {
			String output;
			exporterController.Export(selectedFile, out output);
			WriteLine(output);
		};

		ExportAllAction.Activated += OnExport;


	}

	private void _initToolsColumn ()
	{
		AddFileButton.Clicked += OnFileAddedClicked;
		RemoveButton.Clicked += OnRemoveFile;
		ImportButton.Clicked += OnImportClicked;


		ExportButton.Clicked += OnExport;

		System.EventHandler exportSelected = (sender, e) => {
			String output;
			exporterController.Export(selectedFile, out output);
			WriteLine(output);
		};
		ExportSelectedButton.Clicked += exportSelected; 

	}

	private void _initDragAndDrop ()
	{
		NodeFileList.DragDataReceived += OnDragData;
		Gtk.Drag.DestSet (NodeFileList, DestDefaults.All, target_table, Gdk.DragAction.Copy);
	}

	private void _initNodeStore ()
	{
		CellRendererToggle colorToggle = new Gtk.CellRendererToggle ();
		colorToggle.Toggled += (object o, ToggledArgs args) =>
		{
			VectorFile vf = (VectorFile)(NodeFileList.NodeStore.GetNode (new TreePath (args.Path)));
			vf.ExportColorArt = !vf.ExportColorArt;
		};

		CellRendererToggle lineToggle = new Gtk.CellRendererToggle ();
		lineToggle.Toggled += (object o, ToggledArgs args) =>
		{
			VectorFile vf = (VectorFile)(NodeFileList.NodeStore.GetNode (new TreePath (args.Path)));
			vf.ExportLineArt = !vf.ExportLineArt;
		};

		CellRendererToggle mergedToggle = new Gtk.CellRendererToggle ();
		mergedToggle.Toggled += (object o, ToggledArgs args) =>
		{
			VectorFile vf = (VectorFile)(NodeFileList.NodeStore.GetNode (new TreePath (args.Path)));
			vf.ExportMerged = !vf.ExportMerged;
		};

		CellRendererText optionsRenderer = new CellRendererText ();
		optionsRenderer.Editable = true;
		optionsRenderer.Edited += (object o, EditedArgs args) =>
		{
			VectorFile vf = (VectorFile)(NodeFileList.NodeStore.GetNode (new TreePath (args.Path)));
			vf.Options = args.NewText;

		};

		CellRendererText nameRenderer = new CellRendererText ();
		nameRenderer.Editable = true;
		nameRenderer.Edited += (object o, EditedArgs args) =>
		{
			VectorFile vf = (VectorFile)(NodeFileList.NodeStore.GetNode (new TreePath (args.Path)));
			vf.Name = args.NewText;

		};

		TreeViewColumn nameCol = NodeFileList.AppendColumn ("Name", nameRenderer, "text", 5);
		nameCol.Resizable = true;

		NodeFileList.AppendColumn ("Color", colorToggle, "active", 3);
		NodeFileList.AppendColumn ("Line", lineToggle, "active", 2);
		NodeFileList.AppendColumn ("Merged", mergedToggle, "active", 1);

		TreeViewColumn optionsCol = NodeFileList.AppendColumn ("Options", optionsRenderer, "text", 4);
		optionsCol.Resizable = true;

		NodeFileList.AppendColumn ("Path", new Gtk.CellRendererText (), "text", 0);


		NodeFileList.NodeStore = exportSettingsController.GetListStore ();

		NodeFileList.CursorChanged += RowSelected;

	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}



	protected void OnNewClicked (object sender, EventArgs e)
	{
		Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog ("Create new export settings file.", this, FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Accept);
		fc.SetCurrentFolder (System.IO.Directory.GetCurrentDirectory ());
		FileFilter ff = new FileFilter ();
		ff.AddPattern ("*.tbe");
		fc.Filter = ff;

		if (fc.Run () == (int)ResponseType.Accept)
		{
			String consoleOut;
			exportSettingsController.CreateNewSettings (new Uri (fc.Filename), out consoleOut);
			WriteLine (consoleOut);
		}
		fc.Destroy ();

		UpdateUI ();
	}


	protected void OnOpenClicked (object sender, EventArgs e)
	{
		Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog ("Open existing export settings file.", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
		fc.SetCurrentFolder (System.IO.Directory.GetCurrentDirectory ());
		FileFilter ff = new FileFilter ();
		ff.AddPattern ("*.tbe");
		fc.Filter = ff;

		if (fc.Run () == (int)ResponseType.Accept)
		{
			String consoleOut;
			exportSettingsController.LoadSettings (new Uri (fc.Filename), out consoleOut);
			WriteLine (consoleOut);
		}
		fc.Destroy ();

		UpdateUI ();
	}

	protected void OnSelectExportDestination (object sender, EventArgs e)
	{
		Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog ("Choose directory to export to", this, FileChooserAction.SelectFolder, "Cancel", ResponseType.Cancel, "Select Folder", ResponseType.Accept);

		Console.WriteLine (exportSettingsController.GetSettingsFileLocationUri ());
		fc.SetCurrentFolder (exportSettingsController.GetSettingsFileLocationUri ().AbsolutePath);

		if (fc.Run () == (int)ResponseType.Accept)
		{
			Uri settingsFileLocation = exportSettingsController.GetSettingsFileLocationUri ();
			Uri selectedDirectory = new Uri (fc.Filename);

			Uri relative = settingsFileLocation.MakeRelativeUri (selectedDirectory);

			exportSettingsController.SetOutputPathRelative (relative);
			UpdateExportText ("./" + exportSettingsController.GetOutputPathRelative ());
		}
		fc.Destroy ();
	}

	private void UpdateExportText (String path)
	{
		ExportDestination.Buffer.Text = path;
	}

	private void UpdateUI ()
	{
		GlobalOptionsEntry.Buffer.Text = exportSettingsController.GetGlobalOptions ();
		Title = exportSettingsController.GetSettingsFilename ();
		UpdateExportText ("./" + exportSettingsController.GetOutputPathRelative ());
	}



	public void WriteLine (String text)
	{
		ConsoleText.Buffer.Text += text + "\n";
	}



	protected void OnExport (object sender, EventArgs e)
	{
		String output;
		exporterController.Export (out output);

		WriteLine (output);

	}

	protected void OnSave (object sender, EventArgs e)
	{
		String output;
		exportSettingsController.SaveCurrentSettings (new Uri (exportSettingsController.GetSettingsFileLocationUri () + "./" + exportSettingsController.GetSettingsFilename ()), out output);
		WriteLine (output);
	}

	protected void OnSaveAs (object sender, EventArgs e)
	{
		Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog ("Save export settings as new file.", this, FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Accept);
		fc.SetCurrentFolder (System.IO.Directory.GetCurrentDirectory ());
		FileFilter ff = new FileFilter ();
		ff.AddPattern ("*.tbe");
		fc.Filter = ff;

		if (fc.Run () == (int)ResponseType.Accept)
		{
			String consoleOut;
			exportSettingsController.SaveCurrentSettings (new Uri (fc.Filename), out consoleOut);
			WriteLine (consoleOut);
		}
		fc.Destroy ();
	}

	protected void OnSetToonBoomBinary (object sender, EventArgs e)
	{
		Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog ("Set Toon Boom binary location.", this, FileChooserAction.SelectFolder, "Cancel", ResponseType.Cancel, "Select Folder.", ResponseType.Accept);
		fc.SetCurrentFolder (@"C://Program Files (x86)/Toon Boom Animation/Toon Boom Harmony 12.1 Advanced/win64/bin");

		if (fc.Run () == (int)ResponseType.Accept)
		{
			configController.SetToonBoomBinPath (fc.Filename);

			fc.Destroy ();

			if (configController.Validate ())
			{
				Gtk.MessageDialog dialog = new Gtk.MessageDialog (this, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, "Executables found.");
				dialog.Title = "Success";
				dialog.Run ();
				dialog.Destroy ();
			}
			else
			{
				Gtk.MessageDialog dialog = new Gtk.MessageDialog (this, DialogFlags.Modal, MessageType.Error, ButtonsType.Close, "Executables could not be found.");
				dialog.Title = "Error";
				dialog.Run ();
				dialog.Destroy ();
			}
		}
		else
		{
			fc.Destroy ();
		}

	}


	protected void RowSelected (object sender, EventArgs e)
	{
		TreeSelection selection = (sender as NodeView).Selection;
		NodeStore ns = (sender as NodeView).NodeStore;
		VectorFile vf = ((VectorFile)ns.GetNode (selection.GetSelectedRows () [0]));


		selectedFile = vf;

		Console.WriteLine ("SELECTED");
		/*
		MergedCheck.Toggled -= MergeActivateAction;
		MergedCheck.Active = vf.ExportMerged;
		MergeActivateAction = ((object innerSender, EventArgs e2) =>
		{
			vf.ExportMerged = (innerSender as CheckButton).Active;
		});
		MergedCheck.Toggled+= MergeActivateAction;

		ColorArtCheck.Toggled -= ColorArtActivateAction;
		ColorArtCheck.Active = vf.ExportColorArt;
		ColorArtActivateAction = ((object innerSender, EventArgs e2) =>
		{
				vf.ExportColorArt = (innerSender as CheckButton).Active;
		});
		ColorArtCheck.Toggled+= ColorArtActivateAction;

		LineArtCheck.Toggled -= LineArtActivateAction;
		LineArtCheck.Active = vf.ExportLineArt;
		LineArtActivateAction = ((object innerSender, EventArgs e2) =>
		{
			vf.ExportLineArt = (innerSender as CheckButton).Active;
		});
		LineArtCheck.Toggled+= LineArtActivateAction;

		

		OptionsEntry.Buffer.Text = vf.Options;
		*/
	}

	protected void OnFileAddedClicked (object sender, EventArgs e)
	{
		Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog ("Choose file.", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Select File", ResponseType.Accept);
		fc.SetCurrentFolder (exportSettingsController.GetSettingsFileLocationUri ().AbsolutePath);
		FileFilter ff = new FileFilter ();
		ff.AddPattern ("*.tvg");
		fc.Filter = ff;
		fc.SelectMultiple = true;
		if (fc.Run () == (int)ResponseType.Accept)
		{
			foreach (String filename in fc.Filenames)
			{
				String consoleOut;

				Uri settingsFileLocation = exportSettingsController.GetSettingsFileLocationUri ();

				Uri selectedFile = new Uri (filename);
				Uri relative = settingsFileLocation.MakeRelativeUri (selectedFile);

				exportSettingsController.AddFile (relative, System.IO.Path.GetFileName (filename), out consoleOut);
				WriteLine (consoleOut);
				consoleOut = "";
			}
		}
		fc.Destroy ();
	}

	protected void OnRemoveFile (object sender, EventArgs e)
	{
		if (selectedFile != null)
		{
			String consoleOut;
			exportSettingsController.RemoveFile (selectedFile.FilePath, out consoleOut);
			selectedFile = null;

			WriteLine (consoleOut);
		}

	}

	protected void OnImportClicked (object sender, EventArgs e)
	{
		
		AddFromResourceFileGUI.FilesSelectedCallback callback = (List<VectorFilePath> vfp) =>
		{
			foreach (VectorFilePath entry in vfp)
			{
				String consoleOut;

				Uri settingsFileLocation = exportSettingsController.GetSettingsFileLocationUri ();

				Uri selectedFile = new Uri (entry.Path);
				Uri relative = settingsFileLocation.MakeRelativeUri (selectedFile);

				exportSettingsController.AddFile (relative, entry.Name, out consoleOut);
				WriteLine (consoleOut);
				consoleOut = "";

			}
		};
		resourceGui = new AddFromResourceFileGUI (exportSettingsController, callback);
			
		resourceGui.Run ();
	}


	protected void OnDragData (object o, DragDataReceivedArgs args)
	{
		String filelist = System.Text.Encoding.UTF8.GetString (args.SelectionData.Data);

		foreach (String line in SplitToLines(filelist))
		{
			String cleanLine = line.Replace ("\0", string.Empty);
			if (cleanLine.Length == 0)
			{
				continue;
			}
			String output;

			Console.WriteLine (cleanLine);

			Uri settingsFileLocation = exportSettingsController.GetSettingsFileLocationUri ();

			Uri selectedFile = new Uri (cleanLine);
			Uri relative = settingsFileLocation.MakeRelativeUri (selectedFile);

			exportSettingsController.AddFile (relative, System.IO.Path.GetFileName (cleanLine), out output);

			WriteLine (output);
		}
	}

	public static IEnumerable<String> SplitToLines (String input)
	{
		if (input == null)
		{
			yield break;
		}

		using (System.IO.StringReader reader = new System.IO.StringReader (input))
		{
			String line;
			while ((line = reader.ReadLine ()) != null)
			{
				yield return line;
			}
		}
	}

		
	protected void OnQuit (object sender, EventArgs e)
	{
		Application.Quit ();
	}
}


