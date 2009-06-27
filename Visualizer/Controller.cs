using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ifpfc;
using ifpfc.Logic;
using ifpfc.VM;

namespace Visualizer
{
	internal class Controller : IController
	{
		public Controller(ProblemDescription problem, IVisualizer visualizer)
		{
			vm = new HohmannsEngine(117, problem.ScenarioNumber, problem.ScenarioNumber);
			solverDriver = new Driver(problem.Solver);
			this.visualizer = visualizer;
			simulationTimer.Tick += (sender, args) => StepForward();
			simulationTimer.Start();
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
				return history[currentTime - 1];
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