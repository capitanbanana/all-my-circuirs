using ifpfc.Logic;

namespace Visualizer
{
	internal class ProblemDescription
	{
		public ProblemDescription(int scenarioCount, string imageFile, IProblemSolver solver)
		{
			ScenarioCount = scenarioCount;
			ImageFile = imageFile;
			Solver = solver;
		}

		public readonly int ScenarioCount;
		public readonly string ImageFile;
		public readonly IProblemSolver Solver;
	}
}