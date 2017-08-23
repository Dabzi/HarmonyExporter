using System;
using System.Collections.Generic;
using System.IO;
namespace ToonBoomExportGUI
{
	public class ExportListController
	{
		private List<ExportList> exportLists;
		public ExportList Active { get; private set;}
		private ProjectController parentController;

		private Dictionary<string, FileSystemWatcher> watchers = new Dictionary<string, FileSystemWatcher>();

		public ExportListController (ProjectController parentController)
		{
			exportLists = new List<ExportList> ();
			this.parentController = parentController;
		}

		public void UpdateExportLists (List<ExportList> lists)
		{
			exportLists.Clear ();
			exportLists.AddRange (lists);
			if (!lists.Contains (Active)) {
				if (lists.Count > 0) {
					Active = lists [0];
				} else {
					Active = null;
				}
			}

			foreach (FileSystemWatcher w in watchers.Values) {
				w.EnableRaisingEvents = false;
				w.Dispose();
			}
			watchers.Clear ();

			//Check last modified date vs last export and mark obselete if required.
			foreach (ExportList list in lists) {
				foreach (ElementExportSettings file in list.Elements) {
					string path = new Uri (parentController.FileDirectory, file.FilePath).AbsolutePath;

					DateTime lastModified = System.IO.File.GetLastWriteTimeUtc (path);
					DateTime origin = new DateTime (1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
					TimeSpan diff = lastModified.ToUniversalTime () - origin;
					ulong lastModifiedEpoch= (ulong)(Math.Floor (diff.TotalSeconds));

					if (lastModifiedEpoch > file.LastExport) {
						file.ExportStatus = ElementExportSettings.LastExportStatus.Obsolete;
					}
					string directoryPath = System.IO.Path.GetDirectoryName (path);
					if (!watchers.ContainsKey (directoryPath)) {
						FileSystemWatcher watcher = new FileSystemWatcher (System.IO.Path.GetDirectoryName (path), "*.tvg");
						watchers.Add (directoryPath, watcher);
						watcher.NotifyFilter =  NotifyFilters.Attributes |
												NotifyFilters.CreationTime |
												NotifyFilters.FileName |
												NotifyFilters.LastAccess |
												NotifyFilters.LastWrite |
												NotifyFilters.Size |
												NotifyFilters.Security;
						watcher.EnableRaisingEvents = true;
						Console.WriteLine ("Beginning watcher @ {0}", directoryPath);
						watcher.Changed += (sender, e) => {
							Uri changedUri = new Uri(e.FullPath);
							Console.WriteLine (changedUri + " has changed");
							foreach (ExportList listIt in lists) {
								foreach (ElementExportSettings fileIt in list.Elements) {
									Uri pathIt = new Uri (parentController.FileDirectory, fileIt.FilePath);
									Console.WriteLine ("Comparing {0} to {1}",pathIt, changedUri);
									if (changedUri.Equals (pathIt)) {
										fileIt.ExportStatus = ElementExportSettings.LastExportStatus.Obsolete;
										Console.WriteLine ("Same! marking for rebuild {0}", fileIt.Name);
									}
								}
							}
						};


					}
				}
			}
		}

		public void AddTvg (ElementExportSettings file)
		{
			if (Active != null) {
				file.FilePath = parentController.FileLocation.MakeRelativeUri (file.FilePath);
				Active.Elements.Add (file);
				TvgAdded (Active);
			}
		}

		public void RemoveTvg (ElementExportSettings file, ExportList list)
		{
			list.Elements.Remove (file);
		}

		public void SetActiveExportList (ExportList list)
		{
			if (exportLists.Contains (list)) {
				ExportList old = Active;
				Active = list;
				ActiveExportListChanged (old, Active);
			}
		}
		public delegate void ExportListChangeAction (ExportList oldList, ExportList newList);
		public delegate void TvgAddedAction (ExportList list);

		public event ExportListChangeAction ActiveExportListChanged = delegate { };
		public event TvgAddedAction TvgAdded = delegate { };
	}
}

