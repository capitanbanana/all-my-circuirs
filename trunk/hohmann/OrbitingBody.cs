////////////////////////////////////////////////////////////////////
/// OrbitingBody.cs
/// © 2005 Carl Johansen
/// 
/// Base class for orbiting-body classes.
/// 
///  OrbitingBody implements simplified orbits, so it ignores some of the quantities that
///		are normally used to describe orbits.
///   * It assumes that the orbit stays in the ecliptic plane, so inclination is zero.
///   * It does not implement the concept of longitude.
///  
///  It uses the following properties to describe the orbit:
///  Semi-Major Axis, a [ = half the major axis distance, i.e. the body's mean distance from its primary (the Sun) ]
///  Eccentricity, e (non-circular orbits only) [ = ratio of the distance between the foci to the length of the major axis ]
///  Period, P [ = time to complete one revolution ]
///  True Anomaly, v [ = angular distance of a point in an orbit past the point of periapsis ]
///  Days Elapsed [ = the number of days since the body started orbiting ]
///  
/// The unit of measure for distance is the Astronomical Unit (AU) - the mean distance between the Earth and the Sun.
/// The unit of measure for time is Earth days.
////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;

namespace CarlInc.Demos.Hohmann
{
	/// <summary>
	/// Base class for orbiting-body classes.
	/// </summary>
	public class OrbitingBody
	{
		private int orbitalPeriodDays;
		private double semiMajorAxis; 
		private int daysElapsed;
		private double currX, currY;

		public PointF GetCurrXY() { return new PointF((float) currX, (float) currY); }

		public virtual double SemiMajorAxis { 
									  get { return semiMajorAxis ; } 
									  set 
									  { 
										  semiMajorAxis = value; 
										  //For body orbiting the sun, Period (in years) equals sqrt(a ^ 3) (a in AU)
										  orbitalPeriodDays = (int) (Math.Sqrt(semiMajorAxis * semiMajorAxis * semiMajorAxis) * 365); 			
									  } 
									}
		public int OrbitalPeriod { get { return orbitalPeriodDays ; } }

		/// <summary>
		/// The angle between the body and the sun (in radians).
		/// The base method returns the angle for circular orbits, which is simply the 
		///  percentage of the period completed gives the percentage of the circle covered
		///  (therefore, by definition, angle at start of simulation = zero)				
		/// </summary>		
		public virtual double GetTrueAnomaly() 
		{	return ((double) (GetDaysElapsed() % OrbitalPeriod) / OrbitalPeriod) * 2 * Math.PI; 		}

				
		public int GetDaysElapsed() { return daysElapsed; } 

		/// <summary>
		/// Returns a rectangle that bounds the orbit ellipse.  The base method simply returns a square
		///  centered on (0, 0) whose side length is the orbit's major axis length.
		/// </summary>
		public virtual RectangleF OrbitBoundingRectUnrotated
		{
			get
			{	return new RectangleF((float) (-1 * semiMajorAxis), (float) (-1 * -semiMajorAxis),(float) (2 * semiMajorAxis),(float) (2 * semiMajorAxis));
			}
		}

		/// <summary>
		/// Returns the distance from the primary to the body at its current position.  The base method
		///  simply returns the semi-major axis length (which will always be the correct current radius
		///  if the orbit is circular)
		/// </summary>
		public virtual double CurrentRadius(double bodyOffsetAngle) 
		{	
			return semiMajorAxis; 
		}

		public void Init()
		{
			daysElapsed = 0;
			GetCurrPos();
		}

		/// <summary>
		/// Move the body on in its orbit by one day and update the (x,y) position.
		/// Returns the new true anomaly of the body (in radians).
		/// </summary>
		public double AdvanceOneDay()
		{	
			daysElapsed++;
			return GetCurrPos();
		}

		private double GetCurrPos()
		{
			double bodyOffsetAngle = GetTrueAnomaly();
			double currRadius = CurrentRadius(bodyOffsetAngle);

			// since the sun is at (0,0) in our reference frame, we have easy formulae for (x,y)
			currX = -1 * currRadius * Math.Sin(bodyOffsetAngle);
			currY = -1 * currRadius * Math.Cos(bodyOffsetAngle);

			return(bodyOffsetAngle);
		}

	}
}
