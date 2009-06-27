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
		//не используется, всегда предполагается (0, 0)
		public Location Focus;

		public double SemiMajorAxis;
		public double SemiMinorAxis;

		//угол междку главной полуосью и осью Y (положительное направление - против часовой стрелки)
		public double TransformAngle;

		// The periapsis distance is given by a(1 - e), or a - c
		public double PeriapsisDistance
		{
			get
			{
				if (SemiMinorAxis > SemiMajorAxis)
					throw new InvalidOperationException("Малая полуось оказалсь больше главной!");
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

	// Все параметры задаются в системе единиц СИ
	public interface IVisualizerState
	{
		//диаметр видимой части Вселенной, используется для масштабирования картинки
		double UniverseDiameter { get; }
		
		//состояние управляемого спутника
		Sattelite Voyager { get; }

		//сосотяния целей
		IEnumerable<Sattelite> Targets { get; }
	}
}