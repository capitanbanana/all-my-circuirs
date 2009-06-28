using System;
using System.IO;

namespace ifpfc.Logic
{
	public class Driver : IProblemSolverDriver
	{
		public Driver(IProblemSolver solver)
		{
			this.solver = solver;
			File.Delete("driver.txt");
		}

		public bool IsEnd()
		{
			bool end = solver.State != null && solver.State.Score != 0.0;
			if (end)
				Log("{0} {1}", stepCount, solver.State);
			return end;
		}

		public void Log(string fmt, params object[] args)
		{
			File.AppendAllText("driver.txt", string.Format(fmt, args) + Environment.NewLine);
		}

		public Vector RunStep(double[] outPorts)
		{
			solver.ApplyPortsOutput(outPorts);
			if (stepCount++ < 3) return new Vector(0, 0);
			return solver.CalculateDV();
		}

		public IProblemSolver UnderlyingSolver
		{
			get { return solver; }
		}

		private readonly IProblemSolver solver;
		private int stepCount;
	}
}