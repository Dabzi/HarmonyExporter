using System;
using Gtk;
using System.IO;
using System.Xml.Serialization;

namespace ToonBoomExportGUI
{
	class MainClass
	{

		private StartScreen start;


		private ExporterController exporterController;
		private ExportSettingsController exportSettingsController;
		private ConfigController cfgController;

		public static void Main (string [] args)
		{
			if (args.Length > 0) {
				foreach (String arg in args) {
					Console.WriteLine ("Exporting {0}", arg);
					ConfigController cfgController = new ConfigController ();
					ExporterController exporterController = new ExporterController (cfgController);
					ExportSettingsController exportSettingsController = new ExportSettingsController (exporterController);

					String output;
					Uri ExecUri = new Uri (System.Reflection.Assembly.GetEntryAssembly ().GetName ().CodeBase, UriKind.Absolute);
					Uri RelativeUri = new Uri (arg, UriKind.Relative);
					Uri MergedUri = new Uri (ExecUri, RelativeUri);
					Console.WriteLine (MergedUri);

					cfgController = new ConfigController ();
					exporterController = new ExporterController (cfgController);
					exportSettingsController = new ExportSettingsController (exporterController);

					exportSettingsController.LoadSettings (MergedUri, out output);
					exporterController.Export (out output);
					Console.WriteLine (output);

				}
			} else {
				bool newVer = true;

                Application.Init();

				if (newVer) {
					new MainClass (args);

				} else {
					new MainClass (args);
				}

			}
		}
	

		public MainClass(String[] args)
		{
			initControllers ();
			Application.Init ();
			start = new StartScreen (NewFile, OpenFile);
			Application.Run ();
		}

		private void initControllers()
		{
			cfgController = new ConfigController ();
			exporterController = new ExporterController (cfgController);
			exportSettingsController = new ExportSettingsController (exporterController);
		}

		private void OpenFile(Uri uri)
		{
			ProjectController ctrler = ProjectController.LoadProject (uri);
			ProjectWindow window = new ProjectWindow (ctrler);
		}

		private void NewFile(Uri uri)
		{
			ProjectController ctrler = ProjectController.NewProject (uri);
			ProjectWindow window = new ProjectWindow (ctrler);
		}



	}
}
