using System;
using System.Xml.Serialization;
namespace ToonBoomExportGUI
{
	public class ElementExportSettings : Gtk.TreeNode
	{
		public enum CropSetting
		{
			Default,
			None,
			TVG_All,
			TVG_Underlay,
			TVG_Overlay
		}

		public enum LastExportStatus
		{
			None,
			Success,
			Failure,
			Warnings,
			Obsolete
		}

		public ulong LastExport = 0;

		public LastExportStatus ExportStatus = LastExportStatus.Obsolete;

		public string ExportLog;

		[Gtk.TreeNodeValue (Column = 5)]
		public string IconName {
			get {
				switch (ExportStatus) {
				case LastExportStatus.Success:
					return "gtk-apply";
				case LastExportStatus.Failure:
					return "gtk-dialog-error";
				case LastExportStatus.Warnings:
					return "gtk-dialog-warning";
				case LastExportStatus.Obsolete:
					return "gtk-edit";
				default:
					return "";
				}
			}
		}


		[Gtk.TreeNodeValue (Column = 0)]
		[XmlIgnore]
		public String FilePathString {
			get { return FilePath == null ? null : ((Uri)FilePath).OriginalString; }
			set { FilePath = value == null ? null : new Uri (value, UriKind.Relative); }
		}

		public XmlUri FilePath { get; set; }

		[Gtk.TreeNodeValue (Column = 1)]
		public String Name { get; set; }

		[Gtk.TreeNodeValue (Column = 2)]
		public String Options { get; set; }

		[Gtk.TreeNodeValue (Column = 3)]
		public CropSetting CropMode { get; set; }

		[Gtk.TreeNodeValue (Column = 4)]
		[XmlIgnore]
		public String CropSettingString {
			get {
				return CropMode.ToString ();
			}
			set {
				CropMode = (CropSetting)Enum.Parse (typeof (CropSetting), value);
			}
		}

		public ElementExportSettings ()
		{
			Options = "";
		}

	}
}

