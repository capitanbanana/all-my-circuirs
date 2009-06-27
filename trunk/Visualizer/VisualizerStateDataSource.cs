using System.Collections.Generic;
using ifpfc;

namespace Visualizer
{
	internal class VisualizerStateDataSource : IVisualizerState
	{
		private readonly List<Sattelite> sattelites_;

		public VisualizerStateDataSource()
		{
			UniverseDiameter = 1;
			sattelites_ = new List<Sattelite>();
		}

		public void UpdateState(VisualizerState state)
		{
			UniverseDiameter = state.UniverseDiameter;
			Voyager = new Sattelite
			{
				Name = state.Voyager.Name,
				Location = new Location
				{
					X = state.Voyager.Location.x,
					Y = state.Voyager.Location.y,
				},
				//Orbit = new Orbit
				//{
				//    Focus = new Location { X = 0, Y = 0 },
				//    SemiMajorAxis = 0,
				//    SemiMinorAxis = 0,
				//    TransformAngle = 0
				//}
			};
			sattelites_.Clear();
			foreach (var target in state.Targets)
			{
				sattelites_.Add(new Sattelite
				{
					Name = target.Name,
					Location = new Location
					{
						X = target.Location.x,
						Y = target.Location.x,
					},
					//Orbit = new Orbit
					//{
					//    Focus = new Location { X = 0, Y = 0 },
					//    SemiMajorAxis = 0,
					//    SemiMinorAxis = 0,
					//    TransformAngle = 0
					//}
				});
			}
		}
		
		public double UniverseDiameter { get; private set; }

		public Sattelite Voyager { get; private set; }

		public IEnumerable<Sattelite> Targets
		{
			get { return sattelites_.AsReadOnly(); }
		}
	}
}