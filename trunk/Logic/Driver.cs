using System;
using System.IO;

namespace ifpfc.Logic
{
	public class Driver<TState> : IDriver where TState : BasicState, new()
	{
		private readonly IProblemSolver<TState> solver;
		private TState state;
		private int stepCount;

		public Driver(IProblemSolver<TState> solver)
		{
			this.solver = solver;
			File.Delete("driver.txt");
		}

		public bool IsEnd()
		{
			return state != null && state.Score != 0.0;
		}

		public void Log(string fmt, params object[] args)
		{
			File.AppendAllText("driver.txt", string.Format(fmt, args) + Environment.NewLine);
		}

		public Vector RunStep(double[] outPorts)
		{
			state = solver.ApplyPortsOutput(outPorts, state);
			Log("{0} {1}", stepCount, state);
			if (stepCount++ < 2) return new Vector(0, 0);
			return solver.CalculateDV(state);
		}
	}
}