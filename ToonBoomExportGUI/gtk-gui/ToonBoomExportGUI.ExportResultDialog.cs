
// This file has been generated by the GUI designer. Do not modify.
namespace ToonBoomExportGUI
{
	public partial class ExportResultDialog
	{
		private global::Gtk.ScrolledWindow GtkScrolledWindow;

		private global::Gtk.TextView Output;

		private global::Gtk.Button buttonOk;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget ToonBoomExportGUI.ExportResultDialog
			this.WidthRequest = 800;
			this.HeightRequest = 600;
			this.Name = "ToonBoomExportGUI.ExportResultDialog";
			this.Title = global::Mono.Unix.Catalog.GetString("Export Log");
			this.Icon = global::Stetic.IconLoader.LoadIcon(this, "gtk-dialog-info", global::Gtk.IconSize.Menu);
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child ToonBoomExportGUI.ExportResultDialog.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.Output = new global::Gtk.TextView();
			this.Output.CanFocus = true;
			this.Output.Name = "Output";
			this.Output.Editable = false;
			this.GtkScrolledWindow.Add(this.Output);
			w1.Add(this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(w1[this.GtkScrolledWindow]));
			w3.Position = 0;
			// Internal child ToonBoomExportGUI.ExportResultDialog.ActionArea
			global::Gtk.HButtonBox w4 = this.ActionArea;
			w4.Name = "dialog1_ActionArea";
			w4.Spacing = 10;
			w4.BorderWidth = ((uint)(5));
			w4.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseStock = true;
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "gtk-ok";
			this.AddActionWidget(this.buttonOk, -5);
			global::Gtk.ButtonBox.ButtonBoxChild w5 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w4[this.buttonOk]));
			w5.Expand = false;
			w5.Fill = false;
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.DefaultWidth = 1132;
			this.DefaultHeight = 600;
			this.Show();
		}
	}
}