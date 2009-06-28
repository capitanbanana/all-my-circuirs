using ifpfc;

namespace Visualizer
{
	internal interface IController
	{
		void Step(int stepSize);
		VisualizerState CurrentState { get; }
		int TicksSimulated { get; }
		int CurrentTime { get; }
	}
}