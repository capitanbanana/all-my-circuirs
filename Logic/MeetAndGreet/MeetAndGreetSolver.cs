using System;
using System.IO;
using ifpfc.Logic.Hohmann;

namespace ifpfc.Logic.MeetAndGreet
{
	public class MeetAndGreetSolver : BaseSolver<MeetAndGreetState>
	{
		private const double Eps = 0.01;
		private HohmannAlgoState algoState = HohmannAlgoState.ReadyToJump;
		
		protected override void FinishStateInitialization(double[] outPorts, MeetAndGreetState newState)
		{
			newState.ST = new Vector(outPorts[4], outPorts[5]);
			newState.T = newState.S - newState.ST;
		}

		public override Vector CalculateDV()
		{
			//проверить, что крутимся в одну сторону

			SolverLogger.Log(string.Format("DistanceToTarget: {0}", s.ST.Len()));

			Vector nextV;
			Vector nextPos;
			Physics.Forecast(s.S, s.V, Vector.Zero, out nextPos, out nextV);

			var r0 = s.CurrentOrbitR;
			var r1 = s.TargetOrbitR;

			if (algoState == HohmannAlgoState.ReadyToJump)
			{
				double tmp = (r0 + r1) / (2 * r1);
				double desiredPhi = Math.PI * (1 - Math.Sqrt((tmp * tmp * tmp)));

				var thetaS = s.S.PolarAngle;
				if (thetaS < 0) thetaS += 2 * Math.PI;
				var thetaT = s.T.PolarAngle;
				if (thetaT < 0) thetaT += 2 * Math.PI;
				var actualPhi = thetaT - thetaS;
				if (actualPhi < 0) actualPhi += 2 * Math.PI;

				SolverLogger.Log(string.Format("DesiredPhi: {0}, ActualPhi: {1}", desiredPhi * 180 / Math.PI,
					actualPhi * 180 / Math.PI));

				if (algoState == HohmannAlgoState.ReadyToJump && Math.Abs(desiredPhi - actualPhi) < Eps)
				{
					SolverLogger.Log(string.Format("My POS: {0}, V: {1}", s.S, s.V));
					SolverLogger.Log(string.Format("Target POS: {0}", s.T));
					algoState = HohmannAlgoState.Jumping;
                    var desirableV = Math.Sqrt(2 * Physics.mu * r1 / (r0 * (r0 + r1)));
					//var dv = GetDV(desirableV);
					return (1 - desirableV / s.V.Len()) * s.V;
				}
			}

			if (algoState == HohmannAlgoState.Jumping && (Math.Abs(r0 - r1) < 100))
			{
				algoState = HohmannAlgoState.Finishing;
				//var desirableV = GetDvForSecondJump(nextPos.Len());
				//var dv = GetDV(desirableV);
				//return dv;
				var desirableV = Math.Sqrt(Physics.mu / r1);
				return (1 - desirableV / s.V.Len()) * s.V;
			}
			
			return new Vector(0, 0);
		}

		private Vector GetDV(double desirableV)
		{
			Vector nextV;
			Vector nextPos;
			Physics.Forecast(s.S, s.V, Vector.Zero, out nextPos, out nextV);
			var nextR = nextPos.Len();
			var desirableVector = new Vector(desirableV * nextPos.y / nextR, -desirableV * nextPos.x / nextR);
			SolverLogger.Log("DesirableVector: " + desirableVector.x + ", " + desirableVector.y);
			var vector = new Vector(nextV.x - desirableVector.x, nextV.y - desirableVector.y);
			SolverLogger.Log("DV: " + vector.x + ", " + vector.y);
			//return new Vector(0, 0);
			if (s.Fuel < 5 || vector.x * vector.x + vector.y * vector.y < 1) return new Vector(0, 0);
			return vector;
		}

		private static double GetDvForSecondJump(double r)
		{
			return Math.Sqrt(2 * Physics.mu / r);
		}

		private static double GetDvForFirstJump(double r1, double r0)
		{
			return Math.Sqrt(2 * Physics.mu * r1 / (r0 * (r0 + r1)));
		}

		protected override void FillState(VisualizerState state)
		{
			FillStateByCircularOrbit(state, s.TargetOrbitR);
			state.Targets = new[] {new Sattelite("Target", s.T, new Vector(0, 0))};
		}
	}
}