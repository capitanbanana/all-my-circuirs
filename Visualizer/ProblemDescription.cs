using ifpfc.Logic;

namespace Visualizer
{
	internal class ProblemDescription
	{
		public ProblemDescription(string imageFile, IProblemSolver solver, int scenarioNumber)
		{
			ScenarioNumber = scenarioNumber;
			ImageFile = imageFile;
			Solver = solver;
		}

		public readonly int ScenarioNumber;
		public readonly string ImageFile;
		public readonly IProblemSolver Solver;
	}
}