using System;

namespace ifpfc.Logic
{
	public abstract class BaseSolver<TState> : IProblemSolver where TState : LogicState, new()
	{
		protected TState s;

		#region IProblemSolver Members

		public void ApplyPortsOutput(double[] outPorts)
		{
			var newState = new TState {Score = outPorts[0], Fuel = outPorts[1], Sx = outPorts[2], Sy = outPorts[3]};
			if (s != null)
			{
				newState.Vx = newState.Sx - s.Sx;
				newState.Vy = newState.Sy - s.Sy;
			}
			FinishStateInitialization(outPorts, newState);
			s = newState;
		}

		protected abstract void FinishStateInitialization(double[] outPorts, TState newState);

		public abstract Vector CalculateDV();

		public LogicState State
		{
			get { return s; }
		}

		public abstract VisualizerState GetVisualizerState(LogicState state);

		#endregion
	}
}