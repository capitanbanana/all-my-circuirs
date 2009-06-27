using System;
using System.Collections.Generic;

namespace Visualizer
{
	internal class DummySystemState : ISystemState
	{
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
						Y = 100,
					},
					Orbit = new Orbit
					{
						Focus = new Location {X = 0, Y = 0},
						SemiMajorAxis = 200,
						SemiMinorAxis = 100,
						TransformAngle = Math.PI / 4
					}
				};
			}
		}

		public IEnumerable<Sattelite> Targets
		{
			get { return new Sattelite[]{}; }
		}
	}
}