using ifpfc;

namespace Visualizer
{
	internal interface IController
	{
		void Close();
    void Step(int stepSize);
		VisualizerState CurrentState { get; }
		int TicksSimulated { get; }
		int CurrentTime { get; }
	}
}