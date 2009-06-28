using System;
using System.IO;
using ifpfc.Logic.Hohmann;

namespace ifpfc.Logic.MeetAndGreet
{
	public class MeetAndGreetSolver : BaseSolver<MeetAndGreetState>
	{
		private const double Eps = 0.0001;

		private HohmannAlgoState algoState = HohmannAlgoState.ReadyToJump;
		
		protected override void FinishStateInitialization(double[] outPorts, MeetAndGreetState newState)
		{
			newState.Tx = outPorts[4];
			newState.Ty = outPorts[5];
		}

		public override Vector CalculateDV()
		{
			//проверить, что крутимся в одну сторону

			SolverLogger.Log("Current : V = " + s.V + "  POS = " + s.S);
			Vector nextV;
			Vector nextPos;
			Physics.Forecast(s.S, s.V, Vector.Zero, out nextPos, out nextV);
			SolverLogger.Log("Forecast: V = " + nextV + "  POS = " + nextPos);

			var r0 = s.CurrentOrbitR;
			var r1 = s.TargetOrbitR;
			double tmp = (r0 + r1) / (2 * r1);
			double desiredPhi = Math.PI * (1 - Math.Sqrt((tmp * tmp * tmp)));

			var thetaS = Math.Atan2(s.OS.y, s.OS.x);
			var thetaT = Math.Atan2(s.OT.y, s.OT.x);
			var actualPhi = thetaT - thetaS;
			if (actualPhi < 0) actualPhi += 2*Math.PI;

			double desirableV = 0;
			if (algoState == HohmannAlgoState.ReadyToJump && Math.Abs(desiredPhi - actualPhi) < Eps)
			{
				desirableV = GetDvForFirstJump(r1, r0);
				algoState = HohmannAlgoState.Jumping;
				var dv = GetDV(r0, desirableV);
				File.AppendAllText("driver.txt", "IMPULSE 1 " + dv.x + ", " + dv.y + "\r\n");
				return dv;
			}
			
			if ((Math.Abs(r0 - r1) < 1000) && algoState == HohmannAlgoState.Jumping)
			{
				algoState = HohmannAlgoState.Finishing;
				desirableV = GetDvForSecondJump(nextPos.Len()) * 0.71;
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
			state.Targets = new[] {new Sattelite("Target", s.OT, new Vector(0, 0))};
		}
	}
}