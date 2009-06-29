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
			double f, r;
			do
			{
				r = (left + right) / 2;
				f = F(r, r0, r1, c);
				if (f < 0) left = r;
				if (f > 0) right = r;
			} while (Math.Abs(f) > Eps);
			return r;
		}

		private static double F(double r, double r0, double r1, double c)
		{
			var t1 = r + r0;
			var t2 = r + r1;
			return Math.Sqrt(t1 * t1 * t1) + Math.Sqrt(t2 * t2 * t2) - c;
		}
	}
}