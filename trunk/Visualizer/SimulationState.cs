using System;
using ifpfc.Logic;

namespace Visualizer
{
	[Serializable]
	internal class SimulationState
	{
		public readonly int Time;
		public readonly Vector DV;
		public readonly double[] Sensors;

		public SimulationState(int time, Vector dv, double[] sensors)
		{
			Time = time;
			DV = dv;
			Sensors = sensors;
		}
	}
}