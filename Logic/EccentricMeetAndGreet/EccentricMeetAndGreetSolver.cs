using System;
using ifpfc.Logic.MeetAndGreet;

namespace ifpfc.Logic.EccentricMeetAndGreet
{
	public class EccentricMeetAndGreetSolver : BaseSolver<MeetAndGreetState>
	{
		public EccentricMeetAndGreetSolver(int scenarioNumber)
		{
			this.scenarioNumber = scenarioNumber;
		}

		protected override void FinishStateInitialization(double[] outPorts, MeetAndGreetState newState)
		{
			newState.ST = new Vector(outPorts[4], outPorts[5]);
			newState.T = newState.S - newState.ST;
			if (s != null) newState.TV = newState.T - s.T;
		}

		public override Vector CalculateDV()
		{
			double myOrbit = s.S.Len();
			double myOrbitV = Math.Sqrt(Physics.mu / myOrbit);
			double myV = s.V.Len();
//			SolverLogger.Log(string.Format("Gagarin: {0} Orbit={1} V={2} OrbitV={3}", s.S, myOrbit, myV, myOrbitV));
//			SolverLogger.Log(string.Format("ErrV = {0}",
//				s.V.Len() - myOrbitV
//				));
			return new Vector(0, 0);
		}

		protected override void FillState(VisualizerState state)
		{
			int coef;
			switch (scenarioNumber)
			{
				case 3001:
				case 3004:
					coef = 40;
					break;
				case 3002:
					coef = 140;
					break;
				case 3003:
					coef = 540;
					break;
				default:
					throw new Exception("Неизвестный номер сценария " + scenarioNumber);
			}
			state.UniverseDiameter = coef*1000*1000;
			state.Targets = new[] {new Sattelite("Target", s.T, new Vector(0, 0))};
			state.FixedOrbits = new[] { Physics.CalculateOrbit(s.T, s.TV), Physics.CalculateOrbit(s.S, s.V) };
		}


		private readonly int scenarioNumber;
	}
}