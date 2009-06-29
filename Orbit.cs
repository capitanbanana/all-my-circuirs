using System;

namespace ifpfc
{
	public struct Location
	{
		public double X;
		public double Y;
	}

	public struct Orbit
	{
		public double SemiMajorAxis;
		public double SemiMinorAxis;

		//угол междку главной полуосью и осью Y (положительное направление - против часовой стрелки)
		public double TransformAngle;

		// The periapsis distance is given by a(1 - e), or a - c
		public double PeriapsisDistance
		{
			get
			{
				if (SemiMinorAxis > SemiMajorAxis)
					throw new InvalidOperationException("ћала€ полуось оказалсь больше главной!");
				double ba = SemiMinorAxis / SemiMajorAxis;
				double e = Math.Sqrt(1 - ba * ba);
				return SemiMajorAxis * (1 - e);
			}
		}

		public double a { get { return SemiMajorAxis; } }
		public double b { get { return SemiMinorAxis; } }
		public double c { get { return Math.Sqrt(a * a - b * b); } }
		public double e { get { return c / a; } }
		public double p { get { return b * b / a; } }

		//phi in [-pi, pi]
		public double S(double ro, double phi)
		{
			double s0 = Math.PI * a * b;
			if (phi < 0) return s0 - S(ro, -phi);

			double xa = (ro * Math.Cos(phi) - c) / a;
			double s1 = (s0 - ro * ro * Math.Sin(2 * phi)) / 4;
			double s = s1 + (xa * Math.Sqrt(1 - xa * xa) + a * a * Math.Asin(xa)) * b / (2 * a);
			return s;
		}

		public double S(double phi)
		{
			double ro = p / (1 - e * Math.Cos(phi));
			return S(ro, phi);
		}
	}
}