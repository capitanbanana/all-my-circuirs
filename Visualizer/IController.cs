namespace Visualizer
{
	internal interface IController
	{
		void StepForward();
		void StepBackward();
		void SetSimulationMode(SimulationMode mode);
	}
}