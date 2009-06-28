using System;
using System.Linq;

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

				//avk
				//var glen = Physics.mu/s.S.Len2();
				//var alpha = Math.PI + s.S.PolarAngle;
				//var g = new Vector(glen*Math.Cos(alpha), glen*Math.Sin(alpha));
				//newState.V = newState.S - s.S - 0.5 * g;

				var glen = Physics.mu / s.S.Len2();
				var g = glen * s.S.Norm();
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

		public VisualizerState VisualizerState
		{
			get
			{
				var result = new VisualizerState
					{
						Score = s.Score,
						Voyager = new Sattelite("Гагарин", new Vector(s.Sx, s.Sy), new Vector(s.Vx, s.Vy)),
						Targets = Enumerable.Empty<Sattelite>(),
						FixedOrbits = Enumerable.Empty<Orbit>(),
					};
				FillState(result);
				return result;
			}
		}

		protected void FillStateByCircularOrbit(VisualizerState state, double orbitRadius)
		{
			state.UniverseDiameter = 3*orbitRadius;
			state.FixedOrbits =
				new[]
					{
						new Orbit {SemiMajorAxis = orbitRadius, SemiMinorAxis = orbitRadius},
					};
		}

		protected abstract void FillState(VisualizerState state);
	}
}