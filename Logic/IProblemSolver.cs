namespace ifpfc.Logic
{
	public interface IProblemSolver
	{
		void ApplyPortsOutput(double[] outPorts);
		Vector CalculateDV();
		BasicState State { get; }
	}
}