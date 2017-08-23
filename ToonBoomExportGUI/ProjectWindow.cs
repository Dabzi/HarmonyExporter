using System;
using Gtk;
using System.Collections.Generic;

namespace ToonBoomExportGUI
{
	public partial class ProjectWindow : Gtk.Window
	{
		private ProjectController controller;
		private ExportListController exportListController;
		private Dictionary<Widget, ExportList> notebookMap = new Dictionary<Widget, ExportList> ();

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}

		public ProjectWindow (ProjectController controller) :
				base (Gtk.WindowType.Toplevel)
		{
			this.Build ();

			DeleteEvent += OnDeleteEvent;

			Notebook.RemovePage (0);
			NotebookExport.RemovePage (0);

			this.controller = controller;
			exportListController = controller.ExportController;

			this.Maximize ();


			controller.ExportListsChanged += RefreshExportLists;

			controller.XStageProjectListChanged += RefreshXStageProjectList;


			NotebookExport.SwitchPage += (o, args) => {
				Widget w = NotebookExport.GetNthPage ((int)args.PageNum);
				exportListController.SetActiveExportList (notebookMap [w]);
				Console.WriteLine (notebookMap [w].Name);
			};

			AddXStageButton.Clicked += (sender, e) => {
				Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog ("Open xstage file.", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
				fc.SetCurrentFolder (new Uri(controller.FileLocation, "./").AbsolutePath);
				FileFilter ff = new FileFilter ();
				ff.AddPattern ("*.xstage");
				fc.Filter = ff;

				if (fc.Run () == (int)ResponseType.Accept) {
					XStageProject xsp = XStageProject.Load (fc.Filename);
					List<XStageElement> elementTable = xsp.elements;

					controller.AddXStageProject (new Uri(fc.Uri));
				}
				fc.Destroy ();
			};

			NewExportListButton.Clicked += (sender, e) => {
				Dialog d = new Dialog ("Enter Name", this, DialogFlags.Modal, "Cancel", ResponseType.Cancel, "Ok", ResponseType.Ok);
				Entry nameEntry = new Entry ();
				d.VBox.Add (nameEntry);
				d.ShowAll ();
				nameEntry.Activated += (sender1, e1) => {
					d.Respond (ResponseType.Ok);
				};
				int response = d.Run ();
				if ((ResponseType)response == ResponseType.Ok) {
					controller.AddExportList (nameEntry.Text);
				}
				d.Destroy ();

			};

			_SetupMenu ();
			_SetupExport ();

			Reinit ();

		}

		void RefreshExportLists ()
		{
			//Remove all pages
			while (NotebookExport.NPages > 0) {
				notebookMap.Remove (NotebookExport.GetNthPage (0));
				NotebookExport.RemovePage (0);



			}

			//Recreate pages for all xstage files

			foreach (ExportList exportList in controller.ExportSettings) {
				ExportListTab tab = new ExportListTab (exportList,controller, exportListController);
				Label lbl = new Label (exportList.Name);
				notebookMap.Add (tab, exportList);
				NotebookExport.AppendPage (tab, lbl);



			}
			NotebookExport.ShowAll ();
		}

		void RefreshXStageProjectList ()
		{
			//Remove all pages
			while (Notebook.NPages > 0) {
				Notebook.RemovePage (0);
			}

			//Recreate pages for all xstage files

			foreach (Uri uri in controller.GetXStageProjects ()) {
				Uri absoluteUri = new Uri (controller.FileDirectory, uri);
				XStageProject xstageProj = XStageProject.Load (absoluteUri.AbsolutePath);
				if (xstageProj != null) {
					XStageListTab tab = new XStageListTab (xstageProj, absoluteUri, exportListController);
					Label lbl = new Label (System.IO.Path.GetFileName (absoluteUri.AbsolutePath));

					Notebook.AppendPage (tab, lbl);


				}

			}
			Notebook.ShowAll ();
		}

		private void Reinit ()
		{
			RefreshExportLists ();
			RefreshXStageProjectList ();

			Title = System.IO.Path.GetFileName (controller.FileLocation.AbsolutePath);

		}

		private void _SetupExport ()
		{

			controller.Exporting += (jm) => {
				Root.Sensitive = false;
				GLib.Idle.Add (() => {
					float progress = 0;
					int jobsTotal, jobsDone;
					if (jm.AllJobsDone (out progress, out jobsDone, out jobsTotal)) {
						controller.HandleExportResults (jm.Results);


						ProgressWidget.Fraction = progress;
						ProgressWidget.Text = String.Format ("All jobs done({0})!", jobsDone);
						Root.Sensitive = true;
						return false;
					} else {
						ProgressWidget.Text = String.Format ("{0} / {1} jobs done.", jobsDone, jobsTotal);
						ProgressWidget.Fraction = progress;
						return true;
					}
				});
			};

			BuildButton.Clicked += (sender, e) => {
				ExportJobManager jm = controller.ExportAll ();
			};

			BuildObsolete.Clicked += (sender, e) => {
				ExportJobManager jm = controller.ExportObsolete ();
			};
		}

		private void _SetupMenu ()
		{
			SaveProjectAction.Activated += (sender, e) => {
				controller.Save ();
			};

			SaveProjectAsAction.Activated += (sender, e) => {
				FileChooserDialog fc = new FileChooserDialog ("Save ToonBoom Exporter Project", (Gtk.Window)Toplevel, FileChooserAction.Save,
															 "Cancel", ResponseType.Cancel,
															 "Save", ResponseType.Accept);

				fc.SetCurrentFolder (System.IO.Directory.GetCurrentDirectory ());
				FileFilter ff = new FileFilter ();
				ff.AddPattern ("*.tbp");
				fc.Filter = ff;

				if (fc.Run () == (int)ResponseType.Accept) {
					controller.SaveAs (new Uri (fc.Filename));
					Reinit ();
				}
				fc.Destroy ();

			};


			NewProjectAction.Activated += (sender, e) => {
				FileChooserDialog fc = new FileChooserDialog ("Create new ToonBoom Exporter Project", (Gtk.Window)Toplevel, FileChooserAction.Save,
															 "Cancel", ResponseType.Cancel,
															 "Save", ResponseType.Accept);

				fc.SetCurrentFolder (System.IO.Directory.GetCurrentDirectory ());
				FileFilter ff = new FileFilter ();
				ff.AddPattern ("*.tbp");
				fc.Filter = ff;

				if (fc.Run () == (int)ResponseType.Accept) {
					ProjectController ctrler = ProjectController.NewProject (new Uri (fc.Filename));
					ProjectWindow window = new ProjectWindow (ctrler);

					fc.Destroy ();
					Destroy ();
				} else {
					fc.Destroy ();
				}

			};

			OpenProjectAction.Activated += (sender, e) => {
				Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog ("Open existing ToonBoom Exporter Project.", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
				fc.SetCurrentFolder (System.IO.Directory.GetCurrentDirectory ());
				FileFilter ff = new FileFilter ();
				ff.AddPattern ("*.tbp");
				fc.Filter = ff;

				if (fc.Run () == (int)ResponseType.Accept) {
					ProjectController ctrler = ProjectController.LoadProject (new Uri (fc.Filename));
					ProjectWindow window = new ProjectWindow (ctrler);

					fc.Destroy ();
					Destroy ();
				} else {
					fc.Destroy ();
				}

			};

			SetBinaryLocationAction.Activated += (sender, e) => {
				Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog ("Set Toon Boom binary location.", this, FileChooserAction.SelectFolder, "Cancel", ResponseType.Cancel, "Select Folder.", ResponseType.Accept);
				fc.SetCurrentFolder (@"C://Program Files (x86)/Toon Boom Animation/Toon Boom Harmony 12.2 Advanced/win64/bin");

				if (fc.Run () == (int)ResponseType.Accept) {
					ProjectController.ConfigController.SetToonBoomBinPath (fc.Filename);

					fc.Destroy ();

					if (ProjectController.ConfigController.Validate ()) {
						Gtk.MessageDialog dialog = new Gtk.MessageDialog (this, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, "Executables found.");
						dialog.Title = "Success";
						dialog.Run ();
						dialog.Destroy ();
					} else {
						Gtk.MessageDialog dialog = new Gtk.MessageDialog (this, DialogFlags.Modal, MessageType.Error, ButtonsType.Close, "Executables could not be found.");
						dialog.Title = "Error";
						dialog.Run ();
						dialog.Destroy ();
					}
				} else {
					fc.Destroy ();
				}
			};

		}

	}


}

