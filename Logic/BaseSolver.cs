namespace ifpfc.Logic
{
	public abstract class BaseSolver<TState> : IProblemSolver<TState> where TState : BasicState, new()
	{
		#region IProblemSolver<TState> Members

		public virtual TState ApplyPortsOutput(double[] outPorts, TState oldState)
		{
			var newState = new TState {Score = outPorts[0], Fuel = outPorts[1], Sx = outPorts[2], Sy = outPorts[3]};
			if (oldState != null)
			{
				newState.Vx = newState.Sx - oldState.Sx;
				newState.Vy = newState.Sy - oldState.Sy;
			}
			return newState;
		}

		public abstract Vector CalculateDV(TState state);

		#endregion
	}
}