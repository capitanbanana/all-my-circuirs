using System;
using System.Collections.Generic;
using System.Threading;
using ifpfc;
using ifpfc.Logic;
using ifpfc.Logic.Hohmann;
using ifpfc.VM;

namespace Visualizer
{
	internal class Controller : IController
	{
		public Controller(ProblemDescription problem)
		{
			if (problem.Solver is HohmannSolver)
				vm = new HohmannsEngine(117, problem.ScenarioNumber, problem.ScenarioNumber);
			else if (problem.Solver is MeetAndGreetSolver)
				vm = new MeetAndGreetEngine(117, problem.ScenarioNumber, problem.ScenarioNumber);
			else 
				throw new ArgumentException("неизвестный солвер!");

			solverDriver = new Driver(problem.Solver);
			new Thread(Simulate) { Name = "Симулятор", IsBackground  = true }.Start();
		}
		
		public void Step(int stepSize)
		{
			CurrentTime += stepSize;
			if (CurrentTime < 0)
				CurrentTime = 0;
			if (CurrentTime >= history.Count)
				CurrentTime = history.Count;
		}

		public VisualizerState CurrentState
		{
			get
			{
				int index = CurrentTime - 1;
				if ((index < 0) || (index >= history.Count))
					return null;
				return history[index];
			}
		}

		public int CurrentTime { get; private set; }
		public int TicksSimulated { get; private set; }

		private void Simulate()
		{
			while (TicksSimulated < endOfTheWorld)
			{
				Vector dv = lastSolverOutput ?? Vector.Zero;
				double[] outPorts = vm.RunTimeStep(dv);
				lastSolverOutput = !solverDriver.IsEnd() ? solverDriver.RunStep(outPorts) : Vector.Zero;
				history.Add(solverDriver.UnderlyingSolver.VisualizerState);
				TicksSimulated++;
			}
		}

		private const int endOfTheWorld = 30000000;

		private readonly IList<VisualizerState> history = new List<VisualizerState>();
		private Vector? lastSolverOutput;
		private readonly IVirtualMachine vm;
		private readonly IProblemSolverDriver solverDriver;
	}
}