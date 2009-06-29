using System;
using Xunit;

namespace ifpfc.Logic
{
	public class Physics_Test
	{
		[Fact]
		public void Can_Forecast()
		{
			Vector p, v;
			var oldP = new Vector(7875.21543323545, -6456995.19753615);
			var oldV = new Vector(37.2566558417302, 7814.84948049812);
			Physics.Forecast(oldP, oldV, Vector.Zero, out p, out v);
			Console.WriteLine(oldP);
			Console.WriteLine(oldV);
			Console.WriteLine(p);
			Console.WriteLine(v);
		}

		[Fact]
		public void CalcTempOrbit()
		{
			var orbit = Physics.CalculateTempOrbit(-1, 1000000, 1000001);
			Console.WriteLine(orbit);
		}
	}
}