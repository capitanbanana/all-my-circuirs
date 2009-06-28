namespace ifpfc.Logic
{
	public interface IProblemSolver
	{
		void ApplyPortsOutput(double[] outPorts);
		Vector CalculateDV();
		LogicState State { get; }
		VisualizerState VisualizerState { get; }
	}
}