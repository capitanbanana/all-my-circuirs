using System;
using System.Collections.Generic;
using ifpfc.Logic.MeetAndGreet;

namespace ifpfc.Logic.EccentricMeetAndGreet
{
	public class EccentricMeetAndGreetSolver : BaseSolver<MeetAndGreetState>
	{
		public enum AlgoState
		{
			Initial = 0,
			Destabilized = 1,
			FirstCircular = 2
		}
		private IEnumerator<Vector> dvs;

		public EccentricMeetAndGreetSolver(int scenarioNumber)
		{
			this.scenarioNumber = scenarioNumber;
			dvs = GetSolutions().GetEnumerator();
		}

		private IEnumerable<Vector> GetSolutions()
		{
			yield return Physics.GetStabilizingJump(s.V, s.S);
			yield return Physics.GetStabilizingJump(s.V, s.S);
			var orbit = Physics.CalculateOrbit(s.T, s.TV);
			var polar = Normalize(s.S.PolarAngle);
			var tangle = Normalize(orbit.TransformAngle + Math.PI);


			while (Math.Abs(polar - tangle) > EPS)
			{
				polar = Normalize(s.S.PolarAngle);
				tangle = Normalize(orbit.TransformAngle + Math.PI);
				SolverLogger.Log("Polar = " + polar);
				SolverLogger.Log("Orbit = " + tangle);
				yield return new Vector(0, 0);
			}

			var perigeyDistance = orbit.a * (1 - orbit.e);
			var phi = (s.T.PolarAngle - orbit.TransformAngle);
			var area = orbit.S(phi); //phi * s.T.Len2() / 2;
			var waitTime = area / CalculateH(s.T, s.T - s.TV);
			var tempR = 13736813.23;

			yield return Physics.CalculateHohmannJump(s.S, s.V, tempR);

			polar = Normalize(s.S.PolarAngle);
			tangle = Normalize(orbit.TransformAngle);
			while (Math.Abs(polar - tangle) > EPS)
			{
				polar = Normalize(s.S.PolarAngle);
				tangle = Normalize(orbit.TransformAngle);
				yield return Vector.Zero;
			}
			yield return Physics.CalculateHohmannJump(s.S, s.V, perigeyDistance) * 1.00001;
			polar = Normalize(s.S.PolarAngle);
			tangle = Normalize(orbit.TransformAngle + Math.PI);

			while (Math.Abs(polar - tangle) > 0.01)
			{
				polar = Normalize(s.S.PolarAngle);
				tangle = Normalize(orbit.TransformAngle + Math.PI);
//				SolverLogger.Log("Polar = " + polar);
//				SolverLogger.Log("Orbit = " + tangle);
				yield return new Vector(0, 0);
			}
			SolverLogger.Log("DISTANCE = " + (s.S - s.T).Len());
			var dv = s.V - s.TV;
			SolverLogger.Log("DV = " + dv);
			var d = (s.T.PolarAngle - s.S.PolarAngle);
			SolverLogger.Log("O = " + d);
			var vt = Physics.mu * (1 + orbit.e) / CalculateH(s.T, s.T - s.TV) / 2;
			yield return dv;// *0.999939;
			//yield return s.V - Physics.GetTangentVector(s.S, vt);
			var len = (s.S - s.T).Len();
			int count = 0;
			int maxCount = 0;
			while (true)
			{
				len = (s.S - s.T).Len();
				if (len < 1000)
				{
					d = (s.T.PolarAngle - s.S.PolarAngle);
					SolverLogger.Log("O = " + d);
					SolverLogger.Log("DISTANCE = " + len + "\tcount = " + count + "\t maxCount = " + maxCount+" o=" + d + " MER = " + s.S.Len() + " hisR=" + s.T.Len());
					count++;
				}
				else
				{
					count = 0;
				}
				if (maxCount < count) maxCount = count;
				yield return Vector.Zero;
				
			}
			
		}

		private double Normalize(double a)
		{
			if (a < 0) a += Math.PI * 2;
			if (a >= Math.PI * 2 - EPS) a -= Math.PI * 2;
			return a;
		}

		private double CalculateH(Vector v1, Vector v2)
		{
			var dphi = Math.Abs(v1.PolarAngle - v2.PolarAngle);
			return dphi * (s.T.Len2()) / 2;
		}

		protected override void FinishStateInitialization(double[] outPorts, MeetAndGreetState newState)
		{
			newState.ST = new Vector(outPorts[4], outPorts[5]);
			newState.T = newState.S - newState.ST;
			if (s != null) newState.TV = newState.T - s.T;
		}

		public override Vector CalculateDV()
		{
//			double desirableV = Math.Sqrt(Physics.mu / s.S.Len());
//			SolverLogger.Log("OrbitV = " + desirableV + " RealV = " + s.V.Len());
//			SolverLogger.Log("R = " + s.S.Len());
			//			var targetOrbit = Physics.CalculateOrbit(s.T, s.TV);
			//			SolverLogger.Log("TARGET ORBIT ANGLE = " + targetOrbit.TransformAngle);
			dvs.MoveNext();
			if (dvs.Current.Len() > 0)
			{
				SolverLogger.Log("IMPULSE " + dvs.Current);
			}
			return dvs.Current;
		}

		protected override void FillState(VisualizerState state)
		{
			int coef;
			switch (scenarioNumber)
			{
				case 3001:
				case 3004:
					coef = 40;
					break;
				case 3002:
					coef = 140;
					break;
				case 3003:
					coef = 540;
					break;
				default:
					throw new Exception("Неизвестный номер сценария " + scenarioNumber);
			}
			state.UniverseDiameter = coef*1000*1000;
			state.Targets = new[] {new Sattelite("Target", s.T, new Vector(0, 0))};
			state.FixedOrbits = new[] { Physics.CalculateOrbit(s.T, s.TV), Physics.CalculateOrbit(s.S, s.V) };
		}


		private readonly int scenarioNumber;
		private double EPS = 0.01;
	}
}