using System;
using System.Collections.Generic;

namespace Visualizer
{
	public struct Location
	{
		public double X;
		public double Y;
	}

	public struct Orbit
	{
		public Location Focus;
		public double SemiMajorAxis;
		public double SemiMinorAxis;
		public double TransformAngle;

		// The periapsis distance is given by a(1 - e), or a - c
		public double PeriapsisDistance
		{
			get
			{
				double ba = SemiMinorAxis / SemiMajorAxis;
				double e = Math.Sqrt(1 - ba * ba);
				return SemiMajorAxis * (1 - e);
			}
		}
			
	}

	public struct Sattelite
	{
		public string Name;
		public Location Location;
		public Orbit Orbit;
	}

	public interface ISystemState
	{
		Sattelite Voyager { get; }
		IEnumerable<Sattelite> Targets { get; }
	}
}