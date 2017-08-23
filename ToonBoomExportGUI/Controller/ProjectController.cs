using System;
using System.Collections.Generic;
using Gtk;
using System.Xml.Serialization;
using System.IO;

namespace ToonBoomExportGUI
{
	public class ProjectController
	{

		private ProjectFile pf;
		public Uri FileLocation { get; private set;}
		public Uri FileDirectory {
			get {
				return new Uri (FileLocation, "./");
			}
		}

		public static ConfigController ConfigController = new ConfigController ();
		public ExportListController ExportController { get; private set;}
		public UTransformController UTransformController;

		private static XmlSerializer serializer = new XmlSerializer(typeof(ProjectFile));

		/// <summary>
		/// Creates a new empty project
		/// </summary>
		private ProjectController (ProjectFile file, Uri fileLocation)
		{
			pf = file;
			this.FileLocation = fileLocation;
			ExportController = new ExportListController (this);
			ExportController.UpdateExportLists (file.ExportList);

			UTransformController = new UTransformController (ConfigController);
		}

		public static ProjectController NewProject (Uri fileLocation)
		{
			ProjectFile pf = new ProjectFile ();

			string filename = Path.GetFileNameWithoutExtension (fileLocation.AbsolutePath);
			Uri dir = new Uri (fileLocation, "./");
			fileLocation = new Uri (dir, filename + ".tbp");

			pf.ProjectName = filename;
			pf.ExportList = new List<ExportList> ();
			pf.XStageProjects = new List<XmlUri> ();

			ProjectController.SaveToFile (pf, fileLocation);
			return new ProjectController (pf, fileLocation);

		}

		public static ProjectController LoadProject (Uri fileLocation)
		{
			FileStream fs = File.OpenRead (fileLocation.AbsolutePath);
			ProjectFile pf = ((ProjectFile)serializer.Deserialize (fs));
			fs.Close ();
			if (pf == null) return null;
			return new ProjectController (pf, fileLocation);
		}

		public static void SaveToFile (ProjectFile file, Uri fileLocation)
		{
			FileStream fs = new FileStream (fileLocation.AbsolutePath, FileMode.Create);
			serializer.Serialize (fs, file);
			fs.Close ();
		}

		public void Save ()
		{
			SaveAs (FileLocation);
		}

		public void SaveAs (Uri path)
		{
			FileLocation = path;
			SaveToFile (pf, FileLocation);
		}

		public void AddExportList (String name)
		{
			ExportList newList = new ExportList (name);

			pf.ExportList.Add (newList);

			ExportController.UpdateExportLists (pf.ExportList);

			ExportListsChanged ();
		}

		public void RemoveExportList (ExportList exportList)
		{
			pf.ExportList.Remove (exportList);

			ExportController.UpdateExportLists (pf.ExportList);

			ExportListsChanged ();
		}

		public List<ExportList> ExportSettings {
			get { return pf.ExportList; }
		}


		public void AddXStageProject (Uri uri)
		{
			pf.XStageProjects.Add (FileDirectory.MakeRelativeUri(uri));

			XStageProjectListChanged.Invoke ();
		}

		public List<XmlUri> GetXStageProjects ()
		{
			return pf.XStageProjects;
		}

		public void RemoveXStageProject (Uri uri)
		{
			pf.XStageProjects.Remove (uri);
			XStageProjectListChanged.Invoke ();
		}

		public ExportJobManager ExportAll ()
		{
			ExportJobManager manager = new ExportJobManager ();
			foreach (ExportList list in pf.ExportList) {
				foreach (ElementExportSettings tvg in list.Elements) {
					manager.AddJob (this, list, tvg);
				}
			}
			Exporting (manager);
			manager.RunJobs ();
			return manager;
		}

		public ExportJobManager ExportObsolete ()
		{
			ExportJobManager manager = new ExportJobManager ();
			foreach (ExportList list in pf.ExportList) {
				foreach (ElementExportSettings tvg in list.Elements) {
					if (tvg.ExportStatus == ElementExportSettings.LastExportStatus.Obsolete) {
						manager.AddJob (this, list, tvg);
					}
				}
			}
			Exporting (manager);
			manager.RunJobs ();
			return manager;
		}

		public ExportJobManager Export (ExportList list, List<ElementExportSettings> tvgs)
		{
			ExportJobManager manager = new ExportJobManager ();
			foreach (ElementExportSettings tvg in tvgs) {
				manager.AddJob (this, list, tvg);
			}
			Exporting (manager);
			manager.RunJobs ();
			return manager;
		}

		public void HandleExportResults (List<ExportResult> results)
		{
			foreach (ExportResult result in results) {
				DateTime now = DateTime.UtcNow;
				DateTime origin = new DateTime (1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
				TimeSpan diff = now.ToUniversalTime () - origin;
				ulong lastModifiedEpoch = (ulong)(Math.Floor (diff.TotalSeconds));

				result.Owner.LastExport = lastModifiedEpoch;

				result.Owner.ExportLog = result.FullLog;
				switch (result.Level) {
				case ExportLog.Level.Info:
					result.Owner.ExportStatus = ElementExportSettings.LastExportStatus.Success;
					break;
				case ExportLog.Level.Warning:
					result.Owner.ExportStatus = ElementExportSettings.LastExportStatus.Warnings;
					break;
				case ExportLog.Level.Error:
					result.Owner.ExportStatus = ElementExportSettings.LastExportStatus.Failure;
					break;
				default:
					break;
				}
			}
			Save ();
		}

		public delegate void ExportingNotification (ExportJobManager jobManager);

		public event ExportingNotification Exporting = delegate {};
		public event System.Action XStageProjectListChanged = delegate { };
		public event System.Action ExportListsChanged = delegate { };

	}
}

