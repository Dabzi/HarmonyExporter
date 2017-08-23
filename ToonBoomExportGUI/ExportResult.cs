using System;
using System.Collections.Generic;
using System.Text;
namespace ToonBoomExportGUI
{
	public class ExportResult
	{
		public List<ExportLog> entries = new List<ExportLog> ();
		public ElementExportSettings Owner { get;}

		public ExportLog.Level Level { get; private set;}

		public ExportResult (ElementExportSettings owner)
		{
			Owner = owner;
			Level = ExportLog.Level.Info;
		}

		public void Info (String message, params object[] values)
		{
			entries.Add (new ExportLog (ExportLog.Level.Info, String.Format ("INFO: " + message, values)));
		}

		public void Warn (String message, params object [] values)
		{
			entries.Add (new ExportLog (ExportLog.Level.Warning, String.Format ("WARNING: " + message, values)));
			if (Level == ExportLog.Level.Info) Level = ExportLog.Level.Warning;
		}

		public void Error (String message, params object [] values)
		{
			entries.Add (new ExportLog (ExportLog.Level.Error, String.Format ("ERROR: " + message, values)));
			if (Level != ExportLog.Level.Error) Level = ExportLog.Level.Error;
		}

		public string FullLog {
			get {
				StringBuilder result = new StringBuilder ();
				foreach (ExportLog log in entries) {
					result.AppendLine (log.Message);
				}
				return result.ToString ();
			}
		}
	}

	public class ExportLog
	{
		public enum Level
		{
			Info,
			Warning,
			Error
		}

		public Level MessageLevel;

		public String Message;

		public ExportLog (Level messageLevel, string message)
		{
			MessageLevel = messageLevel;
			Message = message;
		}
	}
}

