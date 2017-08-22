using System;
using Gtk;

namespace ToonBoomExportGUI
{
	public partial class StartScreen : Gtk.Window
	{
		public delegate void NewFile(Uri uri);
		public delegate void OpenFile(Uri uri);

		private NewFile newFileHandler;
		private OpenFile openFileHandler;

		public StartScreen (NewFile newFileHandler, OpenFile openFileHandler) :
			base (Gtk.WindowType.Toplevel)
		{
			this.newFileHandler = newFileHandler;
			this.openFileHandler = openFileHandler;

			this.Build ();
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}

		protected void NewClicked (object sender, EventArgs e)
		{
			Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog ("Create new export settings file.", this, FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Accept);
			fc.SetCurrentFolder(System.IO.Directory.GetCurrentDirectory ());
			FileFilter ff = new FileFilter ();
			ff.AddPattern ("*.tbp");
			fc.Filter = ff;

			if (fc.Run () == (int)ResponseType.Accept) {
				newFileHandler.Invoke (new Uri (fc.Filename));
				fc.Destroy ();
				Destroy ();
			} else {
				fc.Destroy ();
			}


		}

		protected void OpenClicked (object sender, EventArgs e)
		{
			Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog ("Open existing export settings file.", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
			fc.SetCurrentFolder(System.IO.Directory.GetCurrentDirectory ());
			FileFilter ff = new FileFilter ();
			ff.AddPattern ("*.tbp");
			fc.Filter = ff;

			if (fc.Run () == (int)ResponseType.Accept) {
				openFileHandler.Invoke (new Uri (fc.Filename));
				fc.Destroy ();
				Destroy ();
			} else {
				fc.Destroy ();
			}


		}

	}
}

