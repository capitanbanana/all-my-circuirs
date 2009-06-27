using System.Collections.Generic;
using ifpfc.Logic;

namespace ifpfc
{
	public class VisualizerState
	{
		//диаметр видимой части ¬селенной, используетс€ дл€ масштабировани€ картинки
		public double UniverseDiameter;
		
		//состо€ние управл€емого спутника
		public Sattelite Voyager;

		//сосот€ни€ целей
		public IEnumerable<Sattelite> Targets;

		public VisualizerState(double universeDiameter, Sattelite voyager, IEnumerable<Sattelite> targets)
		{
			UniverseDiameter = universeDiameter;
			Voyager = voyager;
			Targets = targets;
		}

		public VisualizerState(Sattelite voyager, IEnumerable<Sattelite> targets)
			: this(1000, voyager, targets)
		{
		}
	}
}