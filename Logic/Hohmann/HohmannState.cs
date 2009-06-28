using System;

namespace ifpfc.Logic.Hohmann
{
	public class HohmannState : LogicState
	{
		public double TargetOrbitR { get; set; }

		public override string ToString()
		{
			
			double r = Math.Sqrt(Sx*Sx + Sy*Sy);
			double v = Math.Sqrt(Vx * Vx + Vy * Vy);
			var error = Math.Sqrt(Physics.mu/r) - v;
			return "TargetR=" + TargetOrbitR +
			       " CurrentR=" + r +
			       " V=" + v + 
                   " ERR=" + error +
			       " Fuel=" + Fuel + " Score=" + Score;
		}
	}

	public enum HohmannAlgoState
	{
		ReadyToJump = 0,
		Jumping,
		Finishing
	}
}