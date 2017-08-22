using System;
using System.Xml.Serialization;
using System.IO;

namespace ToonBoomExportGUI
{
	public class ConfigController
	{
		private XmlSerializer serializer = new XmlSerializer(typeof(Config));
		public Config config { get; private set;}
		public ConfigController ()
		{
			String configLocation = System.IO.Directory.GetCurrentDirectory() + "/config.xml";

			if (System.IO.File.Exists (configLocation))
			{
				FileStream file = System.IO.File.OpenRead (configLocation);

				config = ((Config)serializer.Deserialize (file));

				file.Close ();
			}
			else
			{
				config = new Config ();
				//Attempt to find bin on 64 bit system
				config.ToonBoomDirectoryLocation = @"C://Program Files (x86)/Toon Boom Animation/Toon Boom Harmony 12.1 Advanced/win64/bin";
				if (!config.Validate ())
				{
					//Attempt to find bin on 32 bit system
					config.ToonBoomDirectoryLocation = @"C://Program Files/Toon Boom Animation/Toon Boom Harmony 12.1 Advanced/win32/bin";
					if (!config.Validate ())
					{
						config.ToonBoomDirectoryLocation = @"";
					}
				}

				FileStream file = System.IO.File.OpenWrite (configLocation);

				serializer.Serialize (file, config);

				file.Close ();

			}
		}

		public void SetToonBoomBinPath(String path)
		{
			config.ToonBoomDirectoryLocation = path;

			String configLocation = System.IO.Directory.GetCurrentDirectory() + "/config.xml";
			FileStream file = System.IO.File.Create (configLocation);
			serializer.Serialize (file, config);
			file.Close ();
		}

		public bool Validate()
		{
			return config.Validate ();
		}
	}
}

