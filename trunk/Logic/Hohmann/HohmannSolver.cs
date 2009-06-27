using System;
using System.IO;
using System.Linq;

namespace ifpfc.Logic.Hohmann
{
	public class HohmannSolver : BaseSolver<HohmannState>
	{
		private HohmannAlgoState algoState = HohmannAlgoState.ReadyToJump;
		private int jumpTimeout;
		private int goodTicks;

		protected override void FinishStateInitialization(double[] outPorts, HohmannState newState)
		{
			newState.TargetOrbitR = outPorts[4];
		}

		public override Vector CalculateDV()
		{
			double r0 = Math.Sqrt(s.Sx*s.Sx + s.Sy*s.Sy);
			double v0 = Math.Sqrt(s.Vx*s.Vx + s.Vy*s.Vy);
			double r1 = s.TargetOrbitR;
			if (Math.Abs(r0 - r1) < 1000)
			{
				goodTicks++;
			}
			else
			{
				goodTicks = 0;
			}
			if (goodTicks > 0)
				File.AppendAllText("driver.txt", "GOOD "+ goodTicks + "\r\n");
			double desirableV = 0;
			if (algoState == HohmannAlgoState.ReadyToJump)
			{
				desirableV = GetDvForFirstJump(r1, r0);
				algoState = HohmannAlgoState.Jumping;
				jumpTimeout = 100;
				var dv = GetDV(r0, desirableV);
				File.AppendAllText("driver.txt", "IMPULSE 1 " + dv.x + ", " + dv.y + "\r\n");
				return dv;

			}
			if((Math.Abs(r0 - r1) < 50)) 
				algoState = HohmannAlgoState.Finishing;
			if (algoState == HohmannAlgoState.Finishing)
			{
				algoState = HohmannAlgoState.Finishing;
				desirableV = GetDvForSecondJump(r0);
				var dv = GetDV(r0, desirableV);
				File.AppendAllText("driver.txt", "IMPULSE 2 " + dv.x + ", " + dv.y + "\r\n");
				return dv;
			}
			return new Vector(0, 0);
		}

		private Vector GetDV(double r0, double desirableV)
		{
			Vector desirableVector = new Vector(desirableV * s.Sy / r0, -desirableV * s.Sx / r0);
			var vector = new Vector(s.Vx - desirableVector.x, s.Vy - desirableVector.y);
			if (s.Fuel < 5 || vector.x * vector.x + vector.y * vector.y < 1) return new Vector(0,0);
			return vector;
		}

		public override VisualizerState GetVisualizerState(LogicState state)
		{
			return new VisualizerState(
				new Sattelite(
					"Гагарин",
					new Vector(state.Sx, state.Sy),
					new Vector(state.Vx, state.Vy)),
				Enumerable.Empty<Sattelite>());
		}

		private static double GetDvForSecondJump(double r)
		{
			return Math.Sqrt(2*Physics.mu/r);
		}

		private static double GetDvForFirstJump(double r1, double r0)
		{
			return Math.Sqrt(2*Physics.mu*r1/(r0*(r0 + r1)));
		}
	}
}