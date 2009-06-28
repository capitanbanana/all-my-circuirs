using System;

namespace ifpfc.Logic.MeetAndGreet
{
	public class MeetAndGreetState : LogicState
	{
		public Vector T { get; set; }
		public Vector ST { get; set; }

		public double CurrentOrbitR
		{
			get { return S.Len(); }
		}

		public double TargetOrbitR
		{
			get { return T.Len(); }
		}

		public override string ToString()
		{
			return "TargetR=" + TargetOrbitR + 
			       " CurrentR=" + CurrentOrbitR +
			       " V=" + V.Len() + 
			       " TargetDistance=" + ST.Len() + 
			       " Fuel=" + Fuel + " Score=" + Score;
		}
	}

	public enum MeetAndGreetAlgoState
	{
		Waiting = 0,
		ReadyToJump,
		Jumping,
		Finishing
	}
}