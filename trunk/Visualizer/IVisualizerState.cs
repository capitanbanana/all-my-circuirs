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
		//�� ������������, ������ �������������� (0, 0)
		public Location Focus;

		public double SemiMajorAxis;
		public double SemiMinorAxis;

		//���� ������ ������� �������� � ���� Y (������������� ����������� - ������ ������� �������)
		public double TransformAngle;

		// The periapsis distance is given by a(1 - e), or a - c
		public double PeriapsisDistance
		{
			get
			{
				if (SemiMinorAxis > SemiMajorAxis)
					throw new InvalidOperationException("����� ������� �������� ������ �������!");
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

	// ��� ��������� �������� � ������� ������ ��
	public interface IVisualizerState
	{
		//������� ������� ����� ���������, ������������ ��� ��������������� ��������
		double UniverseDiameter { get; }
		
		//��������� ������������ ��������
		Sattelite Voyager { get; }

		//��������� �����
		IEnumerable<Sattelite> Targets { get; }
	}
}