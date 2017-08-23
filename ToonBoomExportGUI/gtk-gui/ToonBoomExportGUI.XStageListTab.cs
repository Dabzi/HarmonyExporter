
// This file has been generated by the GUI designer. Do not modify.
namespace ToonBoomExportGUI
{
	public partial class XStageListTab
	{
		private global::Gtk.VBox vbox1;

		private global::Gtk.ScrolledWindow GtkScrolledWindow;

		private global::Gtk.NodeView FileTree;

		private global::Gtk.HButtonBox hbuttonbox7;

		private global::Gtk.Button AddButton;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget ToonBoomExportGUI.XStageListTab
			global::Stetic.BinContainer.Attach(this);
			this.Name = "ToonBoomExportGUI.XStageListTab";
			// Container child ToonBoomExportGUI.XStageListTab.Gtk.Container+ContainerChild
			this.vbox1 = new global::Gtk.VBox();
			this.vbox1.Name = "vbox1";
			this.vbox1.Spacing = 6;
			// Container child vbox1.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.FileTree = new global::Gtk.NodeView();
			this.FileTree.CanFocus = true;
			this.FileTree.Name = "FileTree";
			this.GtkScrolledWindow.Add(this.FileTree);
			this.vbox1.Add(this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.GtkScrolledWindow]));
			w2.Position = 0;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbuttonbox7 = new global::Gtk.HButtonBox();
			this.hbuttonbox7.Name = "hbuttonbox7";
			this.hbuttonbox7.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(3));
			// Container child hbuttonbox7.Gtk.ButtonBox+ButtonBoxChild
			this.AddButton = new global::Gtk.Button();
			global::Gtk.Tooltips w3 = new Gtk.Tooltips();
			w3.SetTip(this.AddButton, "Add this file to the currently selected export list.", "Add this file to the currently selected export list.");
			this.AddButton.CanFocus = true;
			this.AddButton.Name = "AddButton";
			this.AddButton.UseUnderline = true;
			global::Gtk.Image w4 = new global::Gtk.Image();
			w4.Pixbuf = global::Stetic.IconLoader.LoadIcon(this, "gtk-add", global::Gtk.IconSize.Menu);
			this.AddButton.Image = w4;
			this.hbuttonbox7.Add(this.AddButton);
			global::Gtk.ButtonBox.ButtonBoxChild w5 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox7[this.AddButton]));
			w5.Expand = false;
			w5.Fill = false;
			this.vbox1.Add(this.hbuttonbox7);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.hbuttonbox7]));
			w6.Position = 1;
			w6.Expand = false;
			w6.Fill = false;
			this.Add(this.vbox1);
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.Hide();
		}
	}
}
