using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace ToonBoomExportGUI
{
	public class ExporterController
	{

		public ExportSettings Settings { get; set;}
		private ConfigController cfgController;

		public ExporterController (ConfigController cfgController)
		{
			this.cfgController = cfgController;
		}

		public void Export (VectorFile f, out String output)
		{
			output = "";

			String[] split = f.Options.Split (' ');


			String args = "";

			foreach(String o in split)
			{
				if (o.Length != 0)
				{
					args += " " + o;
				}
			}

			String[] globalSplit = Settings.GlobalOptions.Split (' ');
			foreach(String o in globalSplit)
			{
				if (o.Length != 0)
				{
					args += " " + o;
				}
			}
				

			String outfileStart = " -outfile \"" + new Uri (Settings.FileLocation, Settings.ExportDirectory).AbsolutePath + "/";

			String outfileEnd =  f.Name + ".png" + "\"";

			String file = " \"" + new Uri (Settings.FileLocation, f.FilePath).AbsolutePath + "\"";



			List<String> execList = new List<String> ();
			if (f.ExportColorArt)
			{
				execList.Add (args + outfileStart + "color_" + outfileEnd + " -nolineart" + file);
			}
			if (f.ExportLineArt)
			{
				execList.Add (args + outfileStart + "line_" + outfileEnd + " -nocolorart" + file);
			}
			if (f.ExportMerged)
			{
				execList.Add (args +outfileStart + outfileEnd + file);
			}
			foreach (String execArgs in execList)
			{
				Process p = new Process ();
				ProcessStartInfo pci = new ProcessStartInfo (cfgController.config.ToonBoomDirectoryLocation + "/utransform.exe", execArgs);

				output += "Executing: " + cfgController.config.ToonBoomDirectoryLocation + "/utransform.exe " + execArgs + "\n";

				pci.RedirectStandardOutput = true;
				pci.RedirectStandardError = true;
				pci.CreateNoWindow = true; 
				pci.UseShellExecute = false;
				p.StartInfo = pci;
				String asyncOut = "";
				p.OutputDataReceived += (object sender, DataReceivedEventArgs e) => asyncOut += e.Data+"\n";
				p.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => asyncOut += e.Data+"\n";
				p.Start ();
				p.BeginOutputReadLine ();
				p.BeginErrorReadLine ();

				p.WaitForExit ();
				output += asyncOut;
			}
		}

		public bool Export(out String output)
		{
			output = "";
			foreach (VectorFile f in Settings.Files)
			{
				String nestedOutput;
				Export (f, out nestedOutput);
				output += nestedOutput;
			}
			return false;
		}
	}
}

