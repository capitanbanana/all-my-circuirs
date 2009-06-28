using System;
using System.IO;

namespace ifpfc.Logic.Hohmann
{
	public class MeetAndGreetSolver : BaseSolver<MeetAndGreetState>
	{
		private const double Eps = 0.001;

		private HohmannAlgoState algoState = HohmannAlgoState.ReadyToJump;
		private int jumpTimeout;
		private int goodTicks;

		protected override void FinishStateInitialization(double[] outPorts, MeetAndGreetState newState)
		{
			newState.Tx = outPorts[4];
			newState.Ty = outPorts[5];
		}

		public override Vector CalculateDV()
		{
			//проверить, что крутимся в одну сторону

			var r1 = s.CurrentOrbitR;
			var r2 = s.TargetOrbitR;
			double tmp = (r1 + r2) / (2 * r2);
			double desiredPhi = Math.PI * (1 - Math.Sqrt((tmp * tmp * tmp)));

			var thetaS = Math.Atan2(s.S.y, s.S.x);
			var thetaT = Math.Atan2(s.T.y, s.T.x);
			var actualPhi = thetaT - thetaS;
			if (actualPhi < 0) actualPhi += 2*Math.PI;

			double desirableV = 0;
			if (algoState == HohmannAlgoState.ReadyToJump && Math.Abs(desiredPhi - actualPhi) < Eps)
			{
				desirableV = GetDvForFirstJump(r2, r1);
				algoState = HohmannAlgoState.Jumping;
				jumpTimeout = 100;
				var dv = GetDV(r1, desirableV);
				File.AppendAllText("driver.txt", "IMPULSE 1 " + dv.x + ", " + dv.y + "\r\n");
				return dv;
			}

			if (Math.Abs(s.ST.Len()) < 1000)
				goodTicks++;
			else
				goodTicks = 0;
			if (goodTicks > 0)
				File.AppendAllText("driver.txt", "GOOD "+ goodTicks + "\r\n");

			if ((Math.Abs(s.ST.Len()) < 50)) 
				algoState = HohmannAlgoState.Finishing;
			if (algoState == HohmannAlgoState.Finishing)
			{
				desirableV = GetDvForSecondJump(r1);
				var dv = GetDV(r1, desirableV);
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

		public override VisualizerState VisualizerState
		{
			get
			{
				return new VisualizerState(
					3 * s.TargetOrbitR,
					new Sattelite(
						"Гагарин",
						s.S,
						new Vector(s.Vx, s.Vy)
					),
					new []{new Sattelite("Target", s.T, new Vector(0, 0)), },
					new[]
						{
							new Orbit { SemiMajorAxis = s.TargetOrbitR, SemiMinorAxis = s.TargetOrbitR },
						}
				);
			}
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