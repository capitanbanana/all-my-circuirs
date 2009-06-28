using System.Collections.Generic;
using ifpfc.Logic;

namespace ifpfc
{
	public class VisualizerState
	{
		//������� ������� ����� ���������, ������������ ��� ��������������� ��������
		public double UniverseDiameter { get; set; }
		//��������� ������������ ��������
		public Sattelite Voyager { get; set; }
		//��������� �����
		public IEnumerable<Sattelite> Targets { get; set; }
		public IEnumerable<Orbit> FixedOrbits { get; set; }
		public double Score { get; set; }

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