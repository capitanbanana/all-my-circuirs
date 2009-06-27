using System.Collections.Generic;
using ifpfc.Logic;

namespace ifpfc
{
	public class VisualizerState
	{
		//������� ������� ����� ���������, ������������ ��� ��������������� ��������
		public double UniverseDiameter;
		
		//��������� ������������ ��������
		public Sattelite Voyager;

		//��������� �����
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