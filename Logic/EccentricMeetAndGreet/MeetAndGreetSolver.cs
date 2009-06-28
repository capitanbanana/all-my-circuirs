using ifpfc.Logic.MeetAndGreet;

namespace ifpfc.Logic.EccentricMeetAndGreet
{
	public class EccentricMeetAndGreetSolver : BaseSolver<MeetAndGreetState>
	{
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
			FillStateByCircularOrbit(state, s.TargetOrbitR);
			state.Targets = new[] {new Sattelite("Target", s.T, new Vector(0, 0))};
		}
	}
}