////////////////////////////////////////////////////////////////////
/// CircularOrbitingBody.cs
/// © 2005 Carl Johansen
/// 
/// Represents a body in circular orbit about a massive central body
////////////////////////////////////////////////////////////////////

using System;

namespace CarlInc.Demos.Hohmann
{
	/// <summary>
	/// Represents a body in circular orbit about a massive central body
	/// </summary>
	public class CircularOrbitingBody : OrbitingBody
	{
		public CircularOrbitingBody(double radius)
		{
			SemiMajorAxis = radius;	// for circular orbits semi-major axis is just the radius		
			base.Init();
		}
	}
}
