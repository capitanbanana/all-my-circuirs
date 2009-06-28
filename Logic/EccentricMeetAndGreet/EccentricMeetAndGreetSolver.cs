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
		}

		public override Vector CalculateDV()
		{
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
		}

		private readonly int scenarioNumber;
	}
}