////////////////////////////////////////////////////////////////////
/// EllipticalOrbitingBody.cs
/// © 2005 Carl Johansen
/// 
/// Represents a body in elliptical orbit about a massive central body
////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;

namespace CarlInc.Demos.Hohmann
{
	/// <summary>
	/// Represents a body in elliptical orbit about a massive central body
	/// </summary>
	public class EllipticalOrbitingBody : OrbitingBody
	{
		private double semiMinorAxis; // also known as "b"
		private double eccentricity;     // also known as "e"
		private double c;             // there doesn't seem to be a name for the distance from the centre to a focus - it's just called "c"
		private double periapsisDistance; // distance from central body to orbiting body at closest point (needed when drawing the orbit ellipse)

		public override double SemiMajorAxis { get { return base.SemiMajorAxis; } 
												set 
												{	
													base.SemiMajorAxis = value;
													RecalcDerivedQuantities();  // update e, b and periapsis distance - they depend on a and c
												} 
											}
		public double PeriapsisDistance { get { return periapsisDistance ; } }

		/// <summary>
		/// Half the distance between the foci of the orbit ellipse
		/// </summary>
		public double C 
		{ 
			get 
			{ return c ; 	}  
			set 
			{ 
				c = value; 
				RecalcDerivedQuantities(); // update e, b and periapsis distance - they depend on a and c
			} 
		}

		public EllipticalOrbitingBody(double semiMajorAxis, double c)
		{
			SemiMajorAxis = semiMajorAxis;
			C = c; 
			RecalcDerivedQuantities();

			base.Init();
		}

		/// <summary>
		/// The angle between the body and the sun (in radians).
		/// </summary>
		public override double GetTrueAnomaly()
		{ 
			// True Anomaly in this context is the angle between the body and the sun.
			// For elliptical orbits, it's a bit tricky.  The percentage of the period completed is still a key input, but we also need
			//  to apply Kepler's equation (based on the eccentricity) to ensure that we sweep out equal areas in equal times.
			//  This equation is transcendental (ie can't be solved algebraically)
			//  so we either have to use an approximating equation or solve by a numeric method.  My implementation uses 
			//  Newton-Raphson iteration to get an excellent approximate answer (usually in 2 or 3 iterations).

			double ecc = eccentricity;			
			double M = base.GetTrueAnomaly();	// the base implementation returns the "mean anomaly", which
												// assumes a circular orbit.  The rest of our work involves correcting
												// this to get the true "eccentric" anomaly
			double E, E_new, E_old = M + (ecc / 2);
			const double epsilon = 0.0001;
			double bodyAngle;

			//Solve [ 0 = E - e sin(E) - M ] for E using Newton-Raphson: Xn+1 = Xn - [ f(Xn) / f'(Xn) ]
			// E = Eccentric Anomaly, M = Mean Anomaly
			do
			{
				E_new = E_old - (E_old - ecc * Math.Sin(E_old) - M) / (1 - ecc * Math.Cos(E_old));
				E_old = E_new;
			} while(Math.Abs(E_old - E_new) > epsilon);
		
			E = E_new;

			//Solve cos(bodyAngle) = ( cos(E) - e ) / (1 - e cos(E) ) to get the body's angle with the Sun
			bodyAngle = Math.Acos( (Math.Cos(E) - ecc)/(1 - ecc * Math.Cos(E)) );

			// Arccos returns a value between 0 and pi, but when M > pi (ie past halfway point) we
			// take (2pi - angle) to get the solution that lies between pi and 2pi
			int T = OrbitalPeriod, t = GetDaysElapsed() % T;
			if(t > T / 2) bodyAngle = 2.0F * Math.PI - bodyAngle;
			return bodyAngle;
		}

		public override double CurrentRadius(double bodyOffsetAngle)
		{
			// for ellipses there is a formula linking radius from a focus to angle and eccentricity:
			// r =  a(1 - e^2) / (1 + e cos(theta))	
			return (SemiMajorAxis * (1 - eccentricity * eccentricity)) / ( 1 + eccentricity * Math.Cos(bodyOffsetAngle));
		}

		/// <summary>
		/// Returns a rectangle that bounds the orbit ellipse.
		/// </summary>
		public override RectangleF OrbitBoundingRectUnrotated
		{
			get
			{			
				return new RectangleF((float) (-1 * semiMinorAxis), 
					(float) (-1 * periapsisDistance), 
					(float) (2 * semiMinorAxis), 
					(float) (2 * SemiMajorAxis));
			}
		}

		private void RecalcDerivedQuantities()
		{
			// eccentricity is simply the ratio of c to a (showing how fat/thin the ellipse is)
			eccentricity = C / SemiMajorAxis;
			// There is a simple formula linking a, b, and e for any ellipse: b^2 = a^2 * (1 - e^2)
			semiMinorAxis = SemiMajorAxis * Math.Sqrt(1 - eccentricity * eccentricity);
			// The periapsis distance is given by a(1 - e), or a - c
			periapsisDistance = SemiMajorAxis - C;			
		}
	}
}
