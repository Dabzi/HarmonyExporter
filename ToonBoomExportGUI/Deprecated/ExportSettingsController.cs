using System;
using System.Xml.Serialization;
using System.IO;

namespace ToonBoomExportGUI
{
	public class ExportSettingsController
	{
		private ExportSettings currentSettings;
		private ExporterController exporter;
		private XmlSerializer serializer;
		private Gtk.NodeStore nodeStore = new Gtk.NodeStore (typeof(VectorFile));

		public ExportSettingsController (ExporterController exporter)
		{
			this.exporter = exporter;
			serializer = new XmlSerializer (typeof(ExportSettings));
		}

		public void CreateNewSettings(Uri fileUri, out String consoleOutput)
		{
			consoleOutput = "";
			currentSettings = new ExportSettings ();
			currentSettings.FileLocation = new Uri(fileUri.AbsoluteUri);


			Uri directory = new Uri (fileUri, ".");
			currentSettings.ExportDirectory = currentSettings.FileLocation.MakeRelativeUri (new Uri(directory.OriginalString + "output"));


			String consoleOutput2;
			SaveCurrentSettings (fileUri, out consoleOutput2);
			consoleOutput += consoleOutput2;

			exporter.Settings = currentSettings;

			nodeStore.Clear ();
		}


		public void SaveCurrentSettings(Uri fileUri, out String consoleOutput)
		{
			currentSettings.FileLocation = new Uri(fileUri.AbsoluteUri);
			consoleOutput = "";
			FileStream fs = new FileStream (fileUri.LocalPath, FileMode.Create);
			serializer.Serialize (fs, currentSettings);
			fs.Close ();
			consoleOutput += "Saved export settings to " + fileUri.AbsolutePath + ".";
		}

		public bool LoadSettings(Uri fileUri, out String consoleOutput)
		{
			consoleOutput = "";

			FileStream fs = File.OpenRead (fileUri.LocalPath);
			currentSettings =((ExportSettings)serializer.Deserialize (fs));
			fs.Close ();

			currentSettings.FileLocation = new Uri(fileUri.AbsoluteUri);

			nodeStore.Clear ();
			foreach(VectorFile v in currentSettings.Files)
			{
				nodeStore.AddNode (v);
			}

			exporter.Settings = currentSettings;
			return true;
		}

		public void AddFile(Uri path, String name, out String consoleOutput)
		{
			VectorFile toAdd = new VectorFile ();
			toAdd.FilePath = path;
			toAdd.Name = name;

			String absolutePath = new Uri (currentSettings.FileLocation, path).AbsolutePath;

			if (File.Exists (absolutePath)) {
				//Check if this 
				FileInfo newFile = new FileInfo(absolutePath);
				bool alreadyExists = false;
				foreach(VectorFile f in currentSettings.Files)
				{
					if (newFile.FullName.Equals (new FileInfo (new Uri (currentSettings.FileLocation, f.FilePath).AbsolutePath).FullName) )
					{
						alreadyExists = true;
					}
				}

				if (alreadyExists) 
				{
					consoleOutput = "Could not add file: Already listed.";
				}
				else 
				{
					consoleOutput = "Added file.";
					currentSettings.Files.Add (toAdd);
					nodeStore.AddNode (toAdd);
				}
			}
			else 
			{
				consoleOutput = "Could not add file: Does not exist at " + absolutePath;
			}
		}


		public void SetGlobalOptions(String options)
		{
			currentSettings.GlobalOptions = options;
		}

		public String GetGlobalOptions()
		{
			return currentSettings.GlobalOptions;
		}

		public void RemoveFile(Uri path, out String consoleOutput)
		{
			VectorFile file = null;
			foreach(VectorFile f in currentSettings.Files)
			{
				if (f.FilePath.Equals (path))
				{
					file = f;
				}
			}
			if (file != null)
			{
				currentSettings.Files.Remove (file);
				nodeStore.RemoveNode (file);
				consoleOutput = "File removed.";
			}
			else
			{
				consoleOutput = "Could not remove file: File has not been added to this configuration.";
			}

		}
		public void SetOutputPathRelative(Uri path)
		{
			currentSettings.ExportDirectory = path;
		}


		public Gtk.NodeStore GetListStore()
		{
			return nodeStore;
		}

		public Uri GetOutputPathRelative()
		{
			return currentSettings.ExportDirectory;
		}

		public Uri GetSettingsFileLocationUri()
		{
			Uri directory = new Uri (currentSettings.FileLocation, ".");
			return new Uri(directory.OriginalString);
		}

		public String GetSettingsFilename()
		{
			return System.IO.Path.GetFileName(currentSettings.FileLocation.LocalPath);
		}

	}
}

