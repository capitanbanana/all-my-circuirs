using System;
using System.Collections.Generic;

namespace Visualizer
{
	internal class DummySystemState : IVisualizerState
	{
		public double UniverseDiameter
		{
			get { return 1000; }
		}

		public Sattelite Voyager
		{
			get
			{
				return new Sattelite
				{
					Name = "Voyager",
					Location = new Location
					{
						X = 100,
						Y = 30,
					},
					Orbit = new Orbit
					{
						Focus = new Location {X = 0, Y = 0},
						SemiMajorAxis = 200,
						SemiMinorAxis = 100,
						TransformAngle = Math.PI / 6
					}
				};
			}
		}

		public IEnumerable<Sattelite> Targets
		{
			get
			{
				return new[]
				{
					new Sattelite
					{
						Name = "T1",
						Location = new Location
						{
							X = -100,
							Y = 30,
						},
						Orbit = new Orbit
						{
							Focus = new Location {X = 0, Y = 0},
							SemiMajorAxis = 200,
							SemiMinorAxis = 100,
							TransformAngle = 3 * Math.PI / 4
						}
					}
				};
			}
		}
	}
}