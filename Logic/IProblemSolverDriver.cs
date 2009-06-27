namespace ifpfc.Logic
{
	public interface IProblemSolverDriver
	{
		bool IsEnd();
		Vector RunStep(double[] outPorts);
		IProblemSolver UnderlyingSolver { get; }
	}
}