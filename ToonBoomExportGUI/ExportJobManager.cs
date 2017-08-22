using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace ToonBoomExportGUI
{
	public class ExportJobManager
	{
		private List<ExportJob> jobs = new List<ExportJob>();
		private int jobCounter = 0;
		private bool running = false;
		private ConcurrentQueue<ExportResult> ExportLogs = new ConcurrentQueue<ExportResult> ();

		public ExportJobManager ()
		{
		}

		public void RunJobs ()
		{
			running = true;
			int i = 0;
			foreach (ExportJob job in jobs) {
				
				UTransformController controller = new UTransformController (ProjectController.ConfigController, i);
				i++;
				Interlocked.Increment (ref jobCounter);

				ThreadPool.QueueUserWorkItem ((state) => {
					ExportResult result;
					try {
						controller.Export (job.projectController, job.exportList, job.tvg, out result);
					} catch (Exception e) {
						result = new ExportResult (job.tvg);
						result.Error (e.GetType() + "\n" + e.StackTrace + "\n" + e.Message);
					}
					ExportLogs.Enqueue (result);

					Interlocked.Decrement (ref jobCounter);
				});
			}
		}

		public void AddJob (ProjectController projectController, ExportList exportList, TvgFileSetting tvg)
		{
			if (!running) {
				jobs.Add (new ExportJob (projectController, exportList, tvg));
			}

		}

		public bool AllJobsDone (out float progress, out int jobsDone, out int totalJobs)
		{
			int jobCounterCopy = jobCounter;

			totalJobs = jobs.Count;
			jobsDone = (totalJobs - jobCounterCopy);
			progress = (float)(jobsDone) / (float)totalJobs;
			return jobCounterCopy == 0;
		}

		public List<ExportResult> Results {
			get {
				return new List<ExportResult> (ExportLogs);
			}
		}


	}

	public class ExportJob
	{
		
		public ProjectController projectController { get; private set;}
		public ExportList exportList { get; private set; }
		public TvgFileSetting tvg { get; private set; }

		public ExportJob (ProjectController projectController, ExportList exportList, TvgFileSetting tvg)
		{
			this.projectController = projectController;
			this.exportList = exportList;
			this.tvg = tvg;
		}
	}
				                              
}

