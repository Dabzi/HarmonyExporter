using System;
namespace ToonBoomExportGUI
{
	public interface ProgressTracker
	{

		int NewJob ();
		void JobComplete (int id);

	}
}

