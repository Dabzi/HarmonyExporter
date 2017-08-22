using System;

namespace ToonBoomExportGUI
{
	public class Config
	{
		public String ToonBoomDirectoryLocation {get;set;}

		public Config ()
		{
		}

		public bool Validate()
		{
			return System.IO.File.Exists (ToonBoomDirectoryLocation + "/utransform.exe");
		}
	}
}

