using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ifpfc.Logic;
using ifpfc.VM;

namespace Visualizer
{
	internal class Controller : IController
	{
		public Controller(IVirtualMachine vm, IDriver driver, IVisualizer visualizer)
		{
			this.vm = vm;
			this.driver = driver;
			this.visualizer = visualizer;
			simulationTimer.Tick += (sender, args) => StepForward();
		}
		
		public void StepForward()
		{
			CurrentTime++;
		}

		public void StepBackward()
		{
			CurrentTime--;
		}

		public void SetSimulationMode(SimulationMode mode)
		{
			simulationTimer.Enabled = mode == SimulationMode.Automatic;
		}

		private void Simulate()
		{
			Vector dv = (CurrentState != null) ? CurrentState.DV : Vector.Zero;
			double[] outPorts = vm.RunTimeStep(dv);
			Vector newDv = !driver.IsEnd() ? driver.RunStep(outPorts) : Vector.Zero;
			history.Add(new SimulationState(currentTime, newDv, outPorts));
		}

		private int CurrentTime
		{
			get { return currentTime; }
			set
			{
				currentTime = Math.Max(0, value);
				if (currentTime >= history.Count)
					Simulate();
				visualizer.Paint(CurrentState);
			}
		}

		private SimulationState CurrentState
		{
			get
			{
				if (currentTime >= history.Count)
					return null;
				return history[currentTime];
			}
		}

		private readonly Timer simulationTimer = new Timer { Interval = 500 };
		private readonly IList<SimulationState> history = new List<SimulationState>();
		private int currentTime;
		private readonly IVirtualMachine vm;
		private readonly IDriver driver;
		private readonly IVisualizer visualizer;
	}
}