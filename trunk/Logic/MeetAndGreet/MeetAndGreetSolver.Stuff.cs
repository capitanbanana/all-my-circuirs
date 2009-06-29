using System;
using ifpfc.Logic.Hohmann;

namespace ifpfc.Logic.MeetAndGreet
{
	public partial class MeetAndGreetSolver
	{
		private double minDistance_ = Double.MaxValue;
		private double? prevDifference_;

		private double PredictCollision(Vector myDV)
		{
			Vector myPos = s.S;
			Vector myV = s.V;
			Vector targetPos = s.T;
			var modTv = Math.Sqrt(Physics.mu / s.TargetOrbitR);
			Vector targetV = new Vector(targetPos.y, -targetPos.x).Norm() * modTv;
			//векторное произведение [v x pos] должно быть > 0 (подгонка - у нас все время движение против часовой стрелки)
			if (targetV.x * targetPos.y - targetV.y * targetPos.x < 0)
				targetV *= -1;

			//вычисляю полупериод Хофмана - настолько нам нужно заглянуть в будущее
			var targetT = 2 * Math.PI * s.TargetOrbitR / modTv;
			double tmp = (s.CurrentOrbitR + s.TargetOrbitR) / (2 * s.TargetOrbitR);
			var dT = targetT * Math.Sqrt(tmp * tmp * tmp);
			for (int t = 0; t < dT; t++)
			{
				Vector nextPos, nextV;
				Physics.Forecast(myPos, myV, t == 0 ? myDV : Vector.Zero, out nextPos, out nextV);
				myPos = nextPos;
				myV = nextV;
				Physics.Forecast(targetPos, targetV, Vector.Zero, out nextPos, out nextV);
				targetPos = nextPos;
				targetV = nextV;
			}
			var dist = (targetPos - myPos).Len();
			SolverLogger.Log(string.Format("Predicted collision distance: {0:F0}", dist));
			return dist;
		}

		public Vector CalculateDv2()
		{
			//проверить, что крутимся в одну сторону

			Vector nextV;
			Vector nextPos;
			Physics.Forecast(s.S, s.V, Vector.Zero, out nextPos, out nextV);
			var r0 = s.CurrentOrbitR;
			var r1 = s.TargetOrbitR;

			var distance = s.ST.Len();
			if (distance < minDistance_)
			{
				minDistance_ = distance;
				SolverLogger.Log(string.Format("Min DistanceToTarget={0:F0}, CurrentR={1:F0}, TargetR={2:F0}", minDistance_, r0, r1));
			}

			if (algoState == HohmannAlgoState.ReadyToJump)
			{
				double tmp = (r0 + r1) / (2 * r1);
				var tau = Math.Sqrt(tmp * tmp * tmp);
				double desiredPhi = r1 > r0 ? Math.PI * (1 - tau) : Math.PI * (tau - 1);

				var thetaS = s.S.PolarAngle;
				if (thetaS < 0) thetaS += 2 * Math.PI;
				var thetaT = s.T.PolarAngle;
				if (thetaT < 0) thetaT += 2 * Math.PI;
				var actualPhi = r1 > r0 ? thetaS - thetaT : thetaT - thetaS;
				if (actualPhi < 0) actualPhi += 2 * Math.PI;

				bool jump = false;
				var difference = desiredPhi - actualPhi;
				if (prevDifference_.HasValue)
					jump = prevDifference_ * difference < 0;
				prevDifference_ = difference;
				if (jump)
				//if (Math.Abs(desiredPhi - actualPhi) < 0.01)
				{
					//SolverLogger.Log(string.Format("My POS: {0}, V: {1}", s.S, s.V));
					//SolverLogger.Log(string.Format("Target POS: {0}", s.T));

					var desirableV = GetDvForFirstJump(r1, r0);
					var dv = GetDV(desirableV);

					//var desirableV = Math.Sqrt(2 * Physics.mu * r1 / (r0 * (r0 + r1)));
					//var dv = (1 - desirableV / s.V.Len()) * s.V;

					if (PredictCollision(dv) < 1000)
					{
						SolverLogger.Log("IMPULSE 1 " + dv.x + ", " + dv.y + "\r\n");
						algoState = HohmannAlgoState.Jumping;
						return dv;
					}
				}
			}

			if (algoState == HohmannAlgoState.Jumping && (Math.Abs(r0 - r1) < 10))
			//if (algoState == HohmannAlgoState.Jumping && distance < 1000)
			{
				//algoState = HohmannAlgoState.Finishing;
				//var desirableV = GetDvForSecondJump(nextPos.Len()) * 0.71;
				//var dv = GetDV(desirableV);

				algoState = HohmannAlgoState.Finishing;
				var desirableV = Math.Sqrt(Physics.mu / r1);
				var dv = (1 - desirableV / s.V.Len()) * s.V;

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
			//SolverLogger.Log("DesirableVector: " + desirableVector.x + ", " + desirableVector.y);
			var vector = new Vector(nextV.x - desirableVector.x, nextV.y - desirableVector.y);
			//SolverLogger.Log("DV: " + vector.x + ", " + vector.y);
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


	}
}