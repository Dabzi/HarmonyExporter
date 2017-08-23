using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace ToonBoomExportGUI
{
	public class UTransformController
	{
		ConfigController cfgController;
		int id;

		//The coordinate space for a TVG file (within 12 fields in all directtions)
		private const float TVGX1 = -2500, TVGX2 = 2500, TVGY1 = -1875, TVGY2 = 1875;

		private Dictionary<ExportType, string> exportExtensionBindings = new Dictionary<ExportType, string> ()
		{
			{ExportType.PNG4, "png"},
			{ExportType.PNG, "png"},
			{ExportType.OMFJPEG, "jpg"},
			{ExportType.OPT, "opt"},
			{ExportType.OPT1, "opt"},
			{ExportType.OPT3, "opt"},
			{ExportType.OPT4, "opt"},
			{ExportType.PAL, "pal"},
			{ExportType.PDF, "pdf"},
			{ExportType.PSD, "psd"},
			{ExportType.PSD1, "psd"},
			{ExportType.PSD3, "psd"},
			{ExportType.PSD4, "psd"},
			{ExportType.PSDDP3, "psd"},
			{ExportType.PSDDP4, "psd"},
			{ExportType.SCAN, "scan"},
			{ExportType.SGI, "sgi"},
			{ExportType.SGI1, "sgi"},
			{ExportType.SGI3, "sgi"},
			{ExportType.SGI4, "sgi"},
			{ExportType.SGIDP3, "sgi"},
			{ExportType.SGIDP4, "sgi"},
			{ExportType.TGA, "tga"},
			{ExportType.TGA1, "tga"},
			{ExportType.TGA3, "tga"},
			{ExportType.TGA4, "tga"},
			{ExportType.TVG, "tvg"},
			{ExportType.YUV, "yuv"}
		};


		public UTransformController (ConfigController cfgController, int id = 0)
		{
			this.cfgController = cfgController;
			this.id = id;
		}

		public void Tvg2Xml (string command)
		{

			Process p = new Process ();
			ProcessStartInfo pci = new ProcessStartInfo (cfgController.config.ToonBoomDirectoryLocation + "/Tvg2Xml.exe", command);

			pci.RedirectStandardOutput = true;
			pci.RedirectStandardError = true;
			pci.CreateNoWindow = true;
			pci.UseShellExecute = false;
			p.StartInfo = pci;

			string _output = string.Empty, _error = string.Empty;
			Debug.WriteLine (command);
			p.OutputDataReceived += (object sender, DataReceivedEventArgs e) => { Debug.WriteLine (e.Data); _output += e.Data + "\n"; };
			p.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => { Debug.WriteLine (e.Data); _error += e.Data + "\n"; };
			p.Start ();
			p.BeginOutputReadLine ();
			p.BeginErrorReadLine ();

			p.WaitForExit ();
		}

		public void RunCommand (string command, out string output, out string error)
		{
			output = string.Empty;
			error = string.Empty;

			Process p = new Process ();
			ProcessStartInfo pci = new ProcessStartInfo (cfgController.config.ToonBoomDirectoryLocation + "/utransform.exe", command);

			pci.RedirectStandardOutput = true;
			pci.RedirectStandardError = true;
			pci.CreateNoWindow = true;
			pci.UseShellExecute = false;
			p.StartInfo = pci;

			string _output = string.Empty, _error = string.Empty;
			Debug.WriteLine (command);
			p.OutputDataReceived += (object sender, DataReceivedEventArgs e) => { Debug.WriteLine (e.Data); _output += e.Data + "\n"; };
			p.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => { Debug.WriteLine (e.Data); _error += e.Data + "\n"; };
			p.Start ();
			p.BeginOutputReadLine ();
			p.BeginErrorReadLine ();

			p.WaitForExit ();
			error = _error;
			output = _output;
		}

		public float [] GetTvgBounds (ProjectController project, string infile)
		{
			//Prepare -printbox command
			string command = string.Format ("-printbox {0}", infile);

			//Run command
			string output, error;
			RunCommand (command, out output, out error);

			float [] result = new float [4];

			//Parse output and extract box.
			using (var stringReader = new StringReader (output)) {
				string line;
				while((line = stringReader.ReadLine()) != null)
				{
					if (line.Contains ("Box:")) {
						//We are on the correct line - and need to extract the values.
						String[] split = line.Split (' ');
						if (split.Length == 5) {
							result [0] = float.Parse(split [1]);
							result [1] = float.Parse (split [2]);
							result [2] = float.Parse (split [3]);
							result [3] = float.Parse (split [4]);
							Debug.WriteLine ("Parse success!");

							break;
						}
					}
				}
			}

			return result;
		}

		public void Export (ProjectController project, ExportList list, ElementExportSettings tvg, out ExportResult output)
		{
			output = new ExportResult (tvg);
			string commandOut, commandErr;
			try {

				output.Info ("Exporting {0} from {1}...", tvg.Name, list.Name);
				ElementExportSettings.CropSetting CropMode = tvg.CropMode;
				if (CropMode == ElementExportSettings.CropSetting.Default) {
					//We need to identify the crop mode from the global settings.
					CropMode = (ElementExportSettings.CropSetting)Enum.Parse (typeof (ElementExportSettings.CropSetting), list.CropSetting);
				}


				int resX = list.ResolutionX;
				int resY = list.ResolutionY;
				ExportType exportType = list.DefaultExportType;

				String outputFile = String.Format ("{0}/{1}{2}{3}.{4}", new Uri (project.FileDirectory, list.ExportDirectory).AbsolutePath,
												list.Prefix,
												tvg.Name,
												list.Suffix,
												  exportExtensionBindings [exportType]);

				String outformat = String.Format ("-outformat {0}", exportType);
				String outfile = String.Format ("-outfile {0}", outputFile);
				String resolution = String.Format ("-resolution {0} {1}", resX, resY);

				//Clean up global arguments
				String [] split = list.Options.Split (' ');
				String args = "";
				foreach (String o in split) {
					if (o.Length != 0) {
						args += " " + o;
					}
				}
				args += " ";

				//Clean up local arguments
				String [] localSplit = tvg.Options.Split (' ');
				String localArgs = "";
				foreach (String o in localSplit) {
					if (o.Length != 0) {
						localArgs += " " + o;
					}
				}
				localArgs += " ";

				string inputFile = String.Format ("{0}",
											   new Uri (project.FileDirectory, tvg.FilePath).AbsolutePath//Path to project location directory
											  );
				String infile = String.Format ("\"{0}\"",
											   inputFile
											  );
				string infileForExport = infile;

				string disableArtCommands = string.Empty;
				if (list.ExportAllArt == false) {
					if (list.ExportColorArt == false) disableArtCommands += "-nocolorart ";
					if (list.ExportLineArt == false) disableArtCommands += "-nolineart ";
				}

				String commandString = String.Format ("{0} {1} {6}{2}{3}{4}{5}", outformat, outfile, resolution, args, localArgs, infileForExport,disableArtCommands);
				output.Info ("utransform {0}", commandString);
				RunCommand (commandString, out commandOut, out commandErr);

				output.Info (commandOut);
				output.Info (commandErr);

				//Find cropping information.
				int [] cropBox = { 0, 0, resX, resY };
				float [] tvgBox;
				int [] rect = new int[0];

				ImageCropper cropper = new ImageCropper ();
				if (exportType == ExportType.PNG || exportType == ExportType.PNG4 || exportType == ExportType.OMFJPEG || exportType == ExportType.PDF) {
					switch (CropMode) {
					case ElementExportSettings.CropSetting.TVG_All:
						tvgBox = GetTvgBounds (project, infile);
						rect = GetRect (tvgBox, resX, resY, output);

						output.Info ("Cropping box:\nx1: {0}, y1: {1}\nx2: {2}, y2: {3}", rect [0], rect [1], rect [2], rect [3]);

						break;
					case ElementExportSettings.CropSetting.TVG_Underlay:
						string underlayOnlyTvg = MakeTemporaryTvgFile ("CROP_UNDERLAY", "-clearlayers colorart,lineart,overlayart", infile, project);
						tvgBox = GetTvgBounds (project, underlayOnlyTvg);
						rect = GetRect (tvgBox, resX, resY, output);

						output.Info ("Cropping box:\nx1: {0}, y1: {1}\nx2: {2}, y2: {3} (underlay only)", rect [0], rect [1], rect [2], rect [3]);
						break;
						case ElementExportSettings.CropSetting.TVG_Overlay:
						string overlayOnly = MakeTemporaryTvgFile ("CROP_OVERLAY", "-clearlayers colorart,lineart,underlayart", infile, project);
						tvgBox = GetTvgBounds (project, overlayOnly);
						rect = GetRect (tvgBox, resX, resY, output);

						output.Info ("Cropping box:\nx1: {0}, y1: {1}\nx2: {2}, y2: {3} (overlay only)", rect [0], rect [1], rect [2], rect [3]);
						break;
					default:
						break;
					}
				} else {
					output.Info ("Cropping is only supported on PNG, PNG4 and OMFJPEG formats.");
				}

				if (rect.Length == 4) {
                    if(exportType != ExportType.PDF)
                    {
                        cropper.CropImage(outputFile, rect[0], rect[1], rect[2], rect[3]);
                    }
                    else
                    {
                        float[] pdfBox = new float[4];
                        pdfBox[0] = rect[0] / (float)resX;
                        pdfBox[1] = rect[1] / (float)resY;
                        pdfBox[2] = rect[2] / (float)resX;
                        pdfBox[3] = rect[3] / (float)resY;

                        cropper.CropPdf(outputFile, pdfBox[0], pdfBox[1], pdfBox[2], pdfBox[3]);
                    }
				}


			} catch (Exception e) {
				output.Error (e.GetType () + "\n" + e.StackTrace + "\n" + e.Message);
			}
		}

		public string MakeTemporaryTvgFile (string name, string arguments, string infile, ProjectController controller)
		{
			
			string temporaryFileString =  new Uri (controller.FileDirectory, "temp/" + name + "_" + id + ".tvg").AbsolutePath;


			Directory.CreateDirectory (Path.GetDirectoryName (temporaryFileString));
			string command = string.Format ("{0} -outformat TVG -outfile \"{1}\" {2}", arguments, temporaryFileString, infile);

			string output, error;

			RunCommand (command, out output, out error);

			Tvg2Xml (String.Format ("-infile \"{0}\" -outfile \"{1}\"", infile, temporaryFileString + "_source.xml"));
			Tvg2Xml (String.Format ("-infile \"{0}\" -outfile \"{1}\"", temporaryFileString, temporaryFileString+".xml"));

			return temporaryFileString;
		}

		/// <summary>
		/// Calculates the pixel coordinates from a set of Tvg coordinates and the resolution of the image
		/// </summary>
		/// <returns>The rect.</returns>
		/// <param name="tvgBounds">Tvg bounds.</param>
		/// <param name="resx">Full X resoultion of image.</param>
		/// <param name="resy">Full Y resolution of image.</param>
		public int [] GetRect (float[] tvgBounds, int resx, int resy, ExportResult log = null)
		{
			float tvgCanvasWidth = TVGX2 - TVGX1; //5000
			float tvgCanvasHeight = TVGY2 - TVGY1; //3750

			//Offset the coordinates so they exist within a 0 -> DIMENSIONSIZE range.
			float [] offsetTvgBounds = new float [4];
			offsetTvgBounds [0] = tvgBounds [0] - TVGX1;
			offsetTvgBounds [2] = tvgBounds [2] - TVGX1;

			offsetTvgBounds [1] = tvgBounds [1] - TVGY1;
			offsetTvgBounds [3] = tvgBounds [3] - TVGY1;

			//Now we determine the scaling factor (Currently for horizontal FOV fit, maybe update this later to allow both)
			bool horizontalFOV = true;
			float scale = horizontalFOV ? ((float)resx/tvgCanvasWidth) : ((float)resy/tvgCanvasHeight);

			int [] pixelRect = new int [4];
			for (int i = 0; i < 4; i++) {
				pixelRect [i] = (int)Math.Round(offsetTvgBounds[i] * scale);
			}

			//In toonboom Up = positive so we need to swap adjust to account for this.
			pixelRect [1] = resy - pixelRect [1];
			pixelRect [3] = resy - pixelRect [3];
			int tmp = pixelRect [1]; pixelRect [1] = pixelRect [3];pixelRect [3] = tmp;

			//Check for errors
			if (pixelRect [0] < 0) {
				if (log != null) {
					log.Warn ("TVG bound exceeds image range! x1: {0}", pixelRect [0]);
				}
				pixelRect [0] = 0;
			}
			if (pixelRect [1] < 0) {
				if (log != null) {
					log.Warn ("TVG bound exceeds image range! y1: {0}", pixelRect [1]);
				}
				pixelRect [1] = 0;
			}

			if (pixelRect [2] > resx) {
				if (log != null) {
					log.Warn ("TVG bound exceeds image range! x2: {0}", pixelRect [2]);
				}
				pixelRect [2] = 0;
			}

			if (pixelRect [3] > resy) {
				if (log != null) {
					log.Warn ("TVG bound exceeds image range! y2: {0}", pixelRect [3]);
				}
				pixelRect [3] = 0;
			}


			return pixelRect;
		}
	}
}

