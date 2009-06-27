using System;

namespace ifpfc.Logic.Hohmann
{
	public class HohmannState : BasicState
	{
		public double TargetOrbitR { get; set; }

		public override string ToString()
		{
			return "TargetR=" + TargetOrbitR + 
			       " CurrentR=" + Math.Sqrt(Sx*Sx + Sy*Sy) +
			       "xy:" + Sx + ", " + Sy +
			       "Vxy:" + Vx + ", " + Vy +
			       " V=" + Math.Sqrt(Vx * Vx + Vy * Vy) + 
			       " Fuel=" + Fuel + " Score=" + Score;
		}
	}

	public enum HohmannAlgoState
	{
		ReadyToJump = 0,
		Jumping
	}
}