namespace ifpfc.Logic
{
	public interface IProblemSolver<TState> where TState : BasicState, new()
	{
		TState ApplyPortsOutput(double[] outPorts, TState oldState);
		Vector CalculateDV(TState state);
	}
}