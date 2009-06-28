using System.Collections.Generic;
using ifpfc.Logic;

namespace ifpfc
{
	public class VisualizerState
	{
		//������� ������� ����� ���������, ������������ ��� ��������������� ��������
		public readonly double UniverseDiameter;
		
		//��������� ������������ ��������
		public readonly Sattelite Voyager;

		//��������� �����
		public readonly IEnumerable<Sattelite> Targets;

		public readonly IEnumerable<Orbit> FixedOrbits;

		public VisualizerState(double universeDiameter, Sattelite voyager, IEnumerable<Sattelite> targets, IEnumerable<Orbit> fixedOrbits)
		{
			UniverseDiameter = universeDiameter;
			FixedOrbits = fixedOrbits;
			Voyager = voyager;
			Targets = targets;
		}

		public VisualizerState(Sattelite voyager, IEnumerable<Sattelite> targets, IEnumerable<Orbit> fixedOrbits)
			: this(60*1000*1000, voyager, targets, fixedOrbits)
		{
			FixedOrbits = fixedOrbits;
		}

		public override string ToString()
		{
			return string.Format(
				"X = {0}, Y = {1}, Vx = {2}, Vy = {3}",
				Voyager.Location.x,
				Voyager.Location.y,
				Voyager.Speed.x,
				Voyager.Speed.y);
		}
	}
}