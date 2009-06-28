using System;

namespace ifpfc.Logic.Hohmann
{
	public class MeetAndGreetState : LogicState
	{
		public double Tx { get; set; }
		public double Ty { get; set; }

		public Vector S { get { return new Vector(-Sx, -Sy); } }
		public Vector T { get { return new Vector(S.x + Tx, S.y + Ty); } }
		public Vector ST { get { return new Vector(Tx, Ty); } }
		
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