using System;
using NUnit.Framework;

namespace ifpfc.Testing
{
	[TestFixture]
	public class Orbit_Test
	{
		private const double Eps = 1e-5;
		
		[Test]
		public void CircularSector()
		{
			var o = new Orbit {SemiMajorAxis = 1, SemiMinorAxis = 1, TransformAngle = 0};
			AssertAreClose(o.S(1, 0), Math.PI / 2);
			AssertAreClose(o.S(1, Math.PI), 0);
			AssertAreClose(o.S(1, -Math.PI/2), 3*Math.PI/4);
			AssertAreClose(o.S(1, -Math.PI), Math.PI);
		}

		[Test]
		public void EllipticSector()
		{
			var o = new Orbit { SemiMajorAxis = 2, SemiMinorAxis = 1, TransformAngle = 0 };
			AssertAreClose(o.S(Math.PI), 0);
			AssertAreClose(o.S(0), Math.PI);
			AssertAreClose(o.S(1, -Math.PI), 2 * Math.PI);
		}

		private static void AssertAreClose(double a1, double a2)
		{
			Assert.Less(Math.Abs(a1 - a2), Eps);
		}
	}
}