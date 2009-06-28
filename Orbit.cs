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
		//не используетс€, всегда предполагаетс€ (0, 0)
		public Location Focus;

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
	}
}