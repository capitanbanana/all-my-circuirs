using System;

namespace ifpfc.Logic.EccentricMeetAndGreet
{
	public static class NumericalMethods
	{
		private const double Eps = 1e-3;

		public static double FindTempR(double r0, double r1, double c)
		{
			double left = 0;
			double right = c;
			double f, ro;
			do
			{
				ro = (left + right) / 2;
				f = F(ro, r0, r1, c);
				if (f < 0) left = ro;
				if (f > 0) right = ro;
			} while (Math.Abs(f) > Eps);
			return ro;
		}

		private static double F(double ro, double r0, double r1, double c)
		{
			var t1 = ro + r0;
			var t2 = ro + r1;
			return Math.Sqrt(t1 * t1 * t1) + Math.Sqrt(t2 * t2 * t2) - c;
		}
	}
}