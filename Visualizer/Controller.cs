using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ifpfc.Logic;
using ifpfc.VM;
using SKBKontur.LIT.Core;

namespace Visualizer
{
	internal class Controller : IController
	{
		public Controller(ProblemDescription problem, IVisualizer visualizer)
		{
			vm = new VirtualMachine(117, problem.ScenarioNumber, problem.ScenarioNumber, new File(problem.ImageFile).Content.Data);
			solverDriver = new Driver(problem.Solver);
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
			Vector dv = lastSolverOutput ?? Vector.Zero;
			double[] outPorts = vm.RunTimeStep(dv);
			lastSolverOutput = !solverDriver.IsEnd() ? solverDriver.RunStep(outPorts) : Vector.Zero;
			history.Add(solverDriver.UnderlyingSolver.State);
		}

		private int CurrentTime
		{
			get { return currentTime; }
			set
			{
				currentTime = Math.Max(0, value);
				if (currentTime >= history.Count)
					Simulate();
				visualizer.Render(solverDriver.UnderlyingSolver.GetVisualizerState(CurrentState));
			}
		}

		private LogicState CurrentState
		{
			get
			{
				if (currentTime >= history.Count)
					return null;
				return history[currentTime];
			}
		}

		private readonly Timer simulationTimer = new Timer { Interval = 500 };
		private readonly IList<LogicState> history = new List<LogicState>();
		private int currentTime;
		private Vector? lastSolverOutput;
		private readonly IVirtualMachine vm;
		private readonly IProblemSolverDriver solverDriver;
		private readonly IVisualizer visualizer;
	}
}