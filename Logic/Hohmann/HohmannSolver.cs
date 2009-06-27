using System;
using System.IO;

namespace ifpfc.Logic.Hohmann
{
	internal class HohmannSolver : BaseSolver<HohmannState>
	{
		public override HohmannState ApplyPortsOutput(double[] outPorts, HohmannState oldState)
		{
			var newState = base.ApplyPortsOutput(outPorts, oldState);
			newState.TargetOrbitR = outPorts[4];
			if (oldState != null) newState.ImpulsesDone = oldState.ImpulsesDone;
			return newState;
		}

		public override Vector CalculateDV(HohmannState s)
		{
			double r0 = Math.Sqrt(s.Sx*s.Sx + s.Sy*s.Sy);
			double v0 = Math.Sqrt(s.Vx*s.Vx + s.Vy*s.Vy);
			double r1 = s.TargetOrbitR;
			double dv = 0;
			if (s.ImpulsesDone == 0)
			{
				dv = GetDvForFirstJump(r1, r0, v0);
				File.AppendAllText("driver.txt", "IMPULSE = " + dv + "\r\n");
				s.ImpulsesDone = 1;
			}
			else if (s.ImpulsesDone == 1)
			{
				if (Math.Abs(r0 - r1) < 500)
				{
					dv = GetDvForSecondJump(r0, v0);
					File.AppendAllText("driver.txt", "IMPULSE = " + dv + "\r\n");
				}
			}
			return new Vector(-dv*s.Vx / v0, -dv*s.Vy / v0);
		}

		private static double GetDvForSecondJump(double r1, double v0)
		{
			double vAfter = Math.Sqrt(2*Physics.mu/r1);
			return vAfter-v0;
		}

		private static double GetDvForFirstJump(double r1, double r0, double v0)
		{
			double vAfter = Math.Sqrt(2*Physics.mu*r1/(r0*(r0 + r1)));
			return vAfter-v0;
		}
	}
}