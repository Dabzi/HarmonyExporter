using System;
namespace ToonBoomExportGUI
{
	public class SceneExportSettings
	{
		public String name;

		public int numFrames;

		public SceneExportSettings()
		{
		}
	}

	public class SceneColumnSettings
	{
		public string name;
		public int displayOrder;

		/// <summary>
		/// The id of the element drawn in this column.
		/// </summary>
		public int id;

		/// <summary>
		/// Should this column be visible in the final composition.
		/// </summary>
		public bool visible;
	}
}
