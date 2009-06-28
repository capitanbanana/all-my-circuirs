using System;

namespace ifpfc.Logic
{
	public class Physics
	{
		public const double G = 6.67428e-11;
		public const double M = 6e24;
		public const double mu = G*M;
		public const double R = 6.357e6;

		public static void Forecast(Vector oldP, Vector oldV, Vector dv, out Vector newP, out Vector newV)
		{
			var oldA = CalculateA(oldP);
			newP = oldP + oldV + 0.5*(oldA + dv);
			var newA = CalculateA(newP);
			newV = oldV + dv + 0.5*(oldA + newA);
		}

		public static Vector CalculateA(Vector pos)
		{
			var alen = mu / pos.Len2();
			var phi = Math.PI + pos.PolarAngle;
			return new Vector(alen*Math.Cos(phi), alen*Math.Sin(phi));
		}
	}
}