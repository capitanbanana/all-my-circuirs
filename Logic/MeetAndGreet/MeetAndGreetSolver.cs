using System;
using ifpfc.Logic.Hohmann;

namespace ifpfc.Logic.MeetAndGreet
{
	public class MeetAndGreetSolver : BaseSolver<MeetAndGreetState>
	{
		private const double Eps = 0.0003;
		private HohmannAlgoState algoState = HohmannAlgoState.ReadyToJump;

		protected override void FinishStateInitialization(double[] outPorts, MeetAndGreetState newState)
		{
			newState.ST = new Vector(outPorts[4], outPorts[5]);
			newState.T = newState.S - newState.ST;
		}

		private double PredictCollision(Vector myDV)
		{
			Vector myPos = s.S;
			Vector myV = s.V;
			Vector targetPos = s.T;
			var modTv = Math.Sqrt(Physics.mu / s.TargetOrbitR);
			Vector targetV = new Vector(targetPos.y, -targetPos.x).Norm() * modTv;
			//векторное произведение [v x pos] должно быть > 0 (подгонка - у нас все время движение против часовой стрелки)
			if(targetV.x * targetPos.y - targetV.y * targetPos.x < 0)
				targetV *= -1;

			//вычисляю полупериод Хофмана - настолько нам нужно заглянуть в будущее
			var targetT = 2 * Math.PI * s.TargetOrbitR / modTv;
			double tmp = (s.CurrentOrbitR + s.TargetOrbitR) / (2 * s.TargetOrbitR);
			var dT = targetT * Math.Sqrt(tmp * tmp * tmp);
			for(int t = 0; t < dT; t++)
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

		public override Vector CalculateDV()
		{
			//проверить, что крутимся в одну сторону

			SolverLogger.Log(string.Format("DistanceToTarget: {0}", s.ST.Len()));

			Vector nextV;
			Vector nextPos;
			Physics.Forecast(s.S, s.V, Vector.Zero, out nextPos, out nextV);

			double r0 = s.CurrentOrbitR;
			double r1 = s.TargetOrbitR;

			if (algoState == HohmannAlgoState.ReadyToJump)
			{
				double tmp = (r0 + r1)/(2*r1);
				double desiredPhi = Math.PI*(1 - Math.Sqrt((tmp*tmp*tmp)));

				double thetaS = s.S.PolarAngle;
				if (thetaS < 0) thetaS += 2*Math.PI;
				double thetaT = s.T.PolarAngle;
				if (thetaT < 0) thetaT += 2*Math.PI;
				double actualPhi = thetaS - thetaT;
				if (actualPhi < 0) actualPhi += 2*Math.PI;

				SolverLogger.Log(string.Format("DesiredPhi: {0}, ActualPhi: {1}, Diff=: {2}", desiredPhi*180/Math.PI,
				                               actualPhi*180/Math.PI, desiredPhi - actualPhi));

				if (algoState == HohmannAlgoState.ReadyToJump && Math.Abs(desiredPhi - actualPhi) < Eps)
				{
					SolverLogger.Log(string.Format("My POS: {0}, V: {1}", s.S, s.V));
					SolverLogger.Log(string.Format("Target POS: {0}", s.T));
					algoState = HohmannAlgoState.Jumping;
					double desirableV = Math.Sqrt(2*Physics.mu*r1/(r0*(r0 + r1)));
					//var dv = GetDV(desirableV);
					return (1 - desirableV/s.V.Len())*s.V;
				}
			}

			if (algoState == HohmannAlgoState.Jumping)
				if (((int)s.ST.Len() < 150))
				{
					algoState = HohmannAlgoState.Finishing;
					//var desirableV = GetDvForSecondJump(nextPos.Len());
					//var dv = GetDV(desirableV);
					//return dv;
					double desirableV = Math.Sqrt(Physics.mu/r1);// *1.0001;
					Vector dv = (1 - desirableV/s.V.Len())*s.V;
					return dv;
				}
			if (algoState == HohmannAlgoState.Finishing)
			{
				SolverLogger.Log(string.Format("Gagarin: {0} Orbit={1} V={2} OrbitV={3}", s.S, s.S.Len(), s.V.Len(), Math.Sqrt(Physics.mu / s.S.Len())));
				SolverLogger.Log(string.Format("Target : {0} Orbit={1} V={2}", s.T, s.TargetOrbitR, Math.Sqrt(Physics.mu / s.TargetOrbitR)));
			}

			return new Vector(0, 0);
		}

		protected override void FillState(VisualizerState state)
		{
			FillStateByCircularOrbit(state, s.TargetOrbitR);
			state.Targets = new[] {new Sattelite("Target", s.T, new Vector(0, 0))};
		}
	}
}