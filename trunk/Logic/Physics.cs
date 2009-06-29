using System;

namespace ifpfc.Logic
{
	public class Physics
	{
		public const double G = 6.67428e-11;
		public const double M = 6e24;
		public const double mu = G*M;
		public const double R = 6.357e6;

		public static double CalculateTempOrbit(double waitTime, double rStart, double rTarget)
		{
			var startPeriod = CalculateOrbitPeriod(rStart);
			var directJumpDoubleTime = startPeriod*Math.Pow((rStart + rTarget)/2/rStart, 3.0/2.0);
			var targetPeriod = CalculateOrbitPeriod(rTarget);
			int i = 0;
			while (2 * waitTime < directJumpDoubleTime)
			{
				waitTime += targetPeriod;
				i++;
			}
			Console.WriteLine(i);
			return 2 * rStart * Math.Pow((2 * waitTime - directJumpDoubleTime) / startPeriod, 2.0 / 3.0) - rTarget;
		}

		private static double CalculateOrbitPeriod(double rStart)
		{
			return rStart*rStart/Math.Sqrt(mu*rStart);
		}

		public static Orbit CalculateOrbit(Vector pos, Vector v)
		{
			double alpha = pos.PolarAngle;
			double beta = v.PolarAngle;
			double vAngle = Math.PI/2 - (alpha - beta);
			double vr = v.Len()*Math.Sin(vAngle);
			double vt = v.Len()*Math.Cos(vAngle);
			double H = vt*pos.Len();
			double theta = Math.Atan2(vr, vt - mu/H);
			double orbitAngle = -(theta + alpha - Math.PI);

			double a = (mu*pos.Len())/(2*mu - v.Len2()*pos.Len());
			double e = vr/(mu*Math.Sin(theta)/H);
			double b = a*Math.Sqrt(1 - e*e);
			SolverLogger.Log("H = " + H);
			return new Orbit {SemiMajorAxis = a, SemiMinorAxis = b, TransformAngle = orbitAngle + Math.PI/2};
		}


		public static void Forecast(Vector oldP, Vector oldV, Vector dv, out Vector newP, out Vector newV)
		{
			Vector oldA = CalculateA(oldP);
			newP = oldP + oldV + 0.5*(oldA + dv);
			Vector newA = CalculateA(newP);
			newV = oldV + dv + 0.5*(oldA + newA);
		}

		public static Vector CalculateA(Vector pos)
		{
			double alen = mu/pos.Len2();
			return alen*pos.Norm();

			//avk
			//var phi = Math.PI + pos.PolarAngle;
			//return new Vector(alen*Math.Cos(phi), alen*Math.Sin(phi));
		}
	}
}