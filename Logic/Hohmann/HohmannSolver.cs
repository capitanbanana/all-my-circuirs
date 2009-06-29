using System;
using System.Linq;

namespace ifpfc.Logic.Hohmann
{
	public class HohmannSolver : BaseSolver<HohmannState>
	{
		private HohmannAlgoState algoState = HohmannAlgoState.ReadyToJump;

		protected override void FinishStateInitialization(double[] outPorts, HohmannState newState)
		{
			newState.TargetOrbitR = outPorts[4];
		}

		public override Vector CalculateDV()
		{
			SolverLogger.Log("Current : V = " + s.V + "  POS = " + s.S);
			Vector nextV;
			Vector nextPos;
			Physics.Forecast(s.S, s.V, Vector.Zero, out nextPos, out nextV);
			SolverLogger.Log("Forecast: V = " + nextV + "  POS = " + nextPos);
			double r0 = Math.Sqrt(s.Sx * s.Sx + s.Sy * s.Sy);
			double r1 = s.TargetOrbitR;
			double desirableV;
			if (algoState == HohmannAlgoState.ReadyToJump)
			{
				//desirableV = GetDvForFirstJump(r1, r0);
				//algoState = HohmannAlgoState.Jumping;
				//var dv = GetDV(desirableV);

				//avk
				algoState = HohmannAlgoState.Jumping;
				desirableV = Math.Sqrt(2 * Physics.mu * r1 / (r0 * (r0 + r1)));
				var dv = (1 - desirableV / s.V.Len()) * s.V;
				
				SolverLogger.Log("IMPULSE 1 " + dv.x + ", " + dv.y + "\r\n");
				return dv;
			}
			if((Math.Abs(r0 - r1) < 500) && algoState == HohmannAlgoState.Jumping) 
			{
				algoState = HohmannAlgoState.Finishing;
				desirableV = GetDvForSecondJump(nextPos.Len()) * 0.71;
				var dv = GetDV(desirableV);

				//avk
				//algoState = HohmannAlgoState.Finishing;
				//desirableV = Math.Sqrt(Physics.mu / r0);
				//var desirableVector = new Vector(desirableV * s.Sy / s.S.Len(), -desirableV * s.Sx / s.S.Len());
				//var dv = s.V - desirableVector;

				SolverLogger.Log("IMPULSE 2 " + dv.x + ", " + dv.y + "\r\n");
				return dv;
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
			if (s.Fuel < 5 || vector.x * vector.x + vector.y * vector.y < 1) return new Vector(0,0);
			return vector;
		}

		protected override void FillState(VisualizerState state)
		{
			FillStateByCircularOrbit(state, s.TargetOrbitR, true);
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