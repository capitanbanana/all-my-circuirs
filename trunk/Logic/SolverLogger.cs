using log4net;

namespace ifpfc.Logic
{
	public static class SolverLogger
	{
		public static void Log(string message, params object[] args)
		{
			log.InfoFormat(message, args);
		}

		private static readonly ILog log = LogManager.GetLogger(typeof(SolverLogger));
	}
}