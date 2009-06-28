using System;
using System.IO;
using System.Linq;
using log4net;

namespace ifpfc.Logic.Hohmann
{
	public class HohmannSolver : BaseSolver<HohmannState>
	{
		private HohmannAlgoState algoState = HohmannAlgoState.ReadyToJump;
		private int goodTicks;
		private int jumps = 0;

		protected override void FinishStateInitialization(double[] outPorts, HohmannState newState)
		{
			newState.TargetOrbitR = outPorts[4];
		}

		public override Vector CalculateDV()
		{
			log.Info("Current : V = " + s.V + "  POS = " + s.S);
			Vector nextV;
			Vector nextPos;
			Physics.Forecast(s.S, s.V, Vector.Zero, out nextPos, out nextV);
			log.Info("Forecast: V = " + nextV + "  POS = " + nextPos);
			double r0 = Math.Sqrt(s.Sx * s.Sx + s.Sy * s.Sy);
			double r1 = s.TargetOrbitR;
			double desirableV = 0;
			if (algoState == HohmannAlgoState.ReadyToJump)
			{
				desirableV = GetDvForFirstJump(r1, r0);
				algoState = HohmannAlgoState.Jumping;
				jumps = 0;
				var dv = GetDV(r0, desirableV);
				File.AppendAllText("driver.txt", "IMPULSE 1 " + dv.x + ", " + dv.y + "\r\n");
				return dv;
			}
			if((Math.Abs(r0 - r1) < 1000) && algoState == HohmannAlgoState.Jumping) 
			{
				algoState = HohmannAlgoState.Finishing;
				desirableV = GetDvForSecondJump(nextPos.Len())*0.71;
				var dv = GetDV(r0, desirableV);
				File.AppendAllText("driver.txt", "IMPULSE 2 " + dv.x + ", " + dv.y + "\r\n");
				return dv;
			}
			return new Vector(0, 0);
		}

		private Vector GetDV(double r0, double desirableV)
		{
			Vector nextV;
			Vector nextPos;
			Physics.Forecast(s.S, s.V, Vector.Zero, out nextPos, out nextV);
			var nextR = nextPos.Len();
			var desirableVector = new Vector(desirableV * nextPos.y / nextR, -desirableV * nextPos.x / nextR);
			log.Info("DesirableVector: " + desirableVector.x + ", " + desirableVector.y);
			var vector = new Vector(nextV.x - desirableVector.x, nextV.y - desirableVector.y);
			log.Info("DV: " + vector.x + ", " + vector.y);
			//return new Vector(0, 0);
			if (s.Fuel < 5 || vector.x * vector.x + vector.y * vector.y < 1) return new Vector(0,0);
			return vector;
		}

		private static ILog log = LogManager.GetLogger(typeof (HohmannSolver));
		public override VisualizerState VisualizerState
		{
			get
			{
				return new VisualizerState(
					3*s.TargetOrbitR,
					new Sattelite(
						"Гагарин",
						new Vector(s.Sx, s.Sy),
						new Vector(s.Vx, s.Vy)),
					Enumerable.Empty<Sattelite>(),
					new[]
						{
							new Orbit { SemiMajorAxis = s.TargetOrbitR, SemiMinorAxis = s.TargetOrbitR },
						});
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