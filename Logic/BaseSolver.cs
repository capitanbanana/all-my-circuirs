using System;

namespace ifpfc.Logic
{
	public abstract class BaseSolver<TState> : IProblemSolver where TState : LogicState, new()
	{
		protected TState s;

		public void ApplyPortsOutput(double[] outPorts)
		{
			var newState = new TState {Score = outPorts[0], Fuel = outPorts[1], S = new Vector(outPorts[2], outPorts[3])};
			if (s != null)
			{
				//newS = S + V + 0.5(g + dv)
				//V =  newS - S - 0.5(g + dv)

				var glen = Physics.mu/s.S.Len2();
				var alpha = s.S.PolarAngle;
				var g = new Vector(glen*Math.Cos(alpha), glen*Math.Sin(alpha));
				newState.V = newState.S - s.S - 0.5 * g;
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

		public abstract VisualizerState VisualizerState { get; }
	}
}