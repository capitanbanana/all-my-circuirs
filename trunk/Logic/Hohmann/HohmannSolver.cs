using System;
using System.IO;

namespace ifpfc.Logic.Hohmann
{
	internal class HohmannSolver : BaseSolver<HohmannState>
	{
		private HohmannAlgoState algoState = HohmannAlgoState.ReadyToJump;
		private int jumpTimeout;

		protected override void FinishStateInitialization(double[] outPorts, HohmannState newState)
		{
			newState.TargetOrbitR = outPorts[4];
		}

		public override Vector CalculateDV()
		{
			double r0 = Math.Sqrt(s.Sx*s.Sx + s.Sy*s.Sy);
			double v0 = Math.Sqrt(s.Vx*s.Vx + s.Vy*s.Vy);
			double r1 = s.TargetOrbitR;
			double dv = 0;
			if (algoState == HohmannAlgoState.ReadyToJump)
			{
				if (Math.Abs(r0 - r1) > 100)
				{
					dv = GetDvForFirstJump(r1, r0, v0);
					File.AppendAllText("driver.txt", "IMPULSE = " + dv + " s = " + algoState + "\r\n");
					algoState = HohmannAlgoState.Jumping;
					jumpTimeout = 100;
				}
			}
			else if (algoState == HohmannAlgoState.Jumping)
			{
				var sp = s.Sx*s.Vx + s.Sy*s.Vy;
				var value = sp / r0 / v0;
				if (jumpTimeout <= 0 && Math.Abs(value) < 0.001)
				{
					dv = GetDvForSecondJump(r0, v0);
					File.AppendAllText("driver.txt", "IMPULSE = " + dv + " value = " + value + " s = " + algoState + "\r\n");
					algoState = HohmannAlgoState.ReadyToJump;
				}
				jumpTimeout--;
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