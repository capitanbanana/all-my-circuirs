using System;

namespace ifpfc.Logic.MeetAndGreet
{
	public class MeetAndGreetState : LogicState
	{
		public double Tx { get; set; }
		public double Ty { get; set; }

		public Vector OS { get { return new Vector(-Sx, -Sy); } }
		public Vector OT { get { return new Vector(OS.x + Tx, OS.y + Ty); } }
		public Vector ST { get { return new Vector(Tx, Ty); } }
		
		public double CurrentOrbitR
		{
			get { return OS.Len(); }
		}

		public double TargetOrbitR
		{
			get { return OT.Len(); }
		}

		public override string ToString()
		{
			return "TargetR=" + TargetOrbitR + 
			       " CurrentR=" + CurrentOrbitR +
			       " V=" + Math.Sqrt(Vx * Vx + Vy * Vy) + 
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