////////////////////////////////////////////////////////////////////
/// DemoController.cs
/// © 2005 Carl Johansen
/// 
/// Controls the Hohmann Transfer Orbit demonstration
////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Resources;
using System.Reflection;
using System.Globalization;

namespace CarlInc.Demos.Hohmann
{
	/// <summary>
	/// Controls the Hohmann Transfer Orbit demonstration
	/// </summary>
	public class DemoController
	{
		public delegate void ShowInfoTextDelegate(string infoMessage, bool showContinueButton, bool showResetButton);

		public CircularOrbitingBody bodyEarth, bodyMars;
		public EllipticalOrbitingBody bodySatellite;

		private const double rEarth = 1.0, rMars = 1.5133;

		private bool showingSatellite, freeSpinning;
		private SignWatcher satEventWatcher;
		private Byte currStatus;
		private int simulationDaysElapsed;
		private DateTime simulationCurrDate;
		public DateTime SimulationCurrDate { get { return simulationCurrDate; } }
		
		public double SatMajorAxisAngle;
		
		public double DisplayHeightInAU() { return rMars * 2.0; }

		private RectangleF satOrbitRect;
		public RectangleF SatOrbitRectangle { get { return satOrbitRect; } }

		public bool ShowingSatellite { get { return showingSatellite; } set { showingSatellite = value; } }

		public DemoController()	{ }

		public void Init()
		{
			currStatus = 0; 
			simulationDaysElapsed = 0;
			simulationCurrDate = new DateTime(2001,6,13,0,0,0,0); //Simulation starts with Earth and Mars in
																  //opposition.  This occurred on June 13, 2001

			showingSatellite = false; freeSpinning = false;
			bodyEarth = new CircularOrbitingBody(rEarth);
			bodyMars = new CircularOrbitingBody(rMars);
			bodySatellite = new EllipticalOrbitingBody((bodyEarth.SemiMajorAxis + bodyMars.SemiMajorAxis) / 2, (bodyMars.SemiMajorAxis - bodyEarth.SemiMajorAxis) / 2);
			satOrbitRect = new RectangleF(0, 0, 0, 0);
			satEventWatcher = new SignWatcher();
		}

		public void AdvanceOneDay(ShowInfoTextDelegate ShowInfoText)
		{
			double satAnomaly;

			simulationDaysElapsed++;
			simulationCurrDate = simulationCurrDate.AddDays(1);

			bodyEarth.AdvanceOneDay();
			bodyMars.AdvanceOneDay();

			if(showingSatellite) 
			{
				satAnomaly = bodySatellite.AdvanceOneDay();
				if(!freeSpinning && satEventWatcher.SignChangeInDifference(satAnomaly, Math.PI)) //check for completion of half an orbit (ie arrival on Mars)
				{
					MoveToNextStatus(ShowInfoText, true, true);
					freeSpinning = true;
					return;
				}
			}

			if(!showingSatellite)
			{   //look for launch window
				int Tmars = bodyMars.OrbitalPeriod, Tsat = bodySatellite.OrbitalPeriod;
				//In Tsat/2 days, satellite will travel Pi radians,	and Mars will travel [Tsat/Tmars] * Pi radians,													  
				//so need to launch when Mars has a headstart of Pi (1- [Tsat/Tmars]) radians on the Earth
				double marsHeadstartRequired = Math.PI * (1 - (double) Tsat / Tmars);
				double earthAngle = bodyEarth.GetTrueAnomaly(),	marsAngle = bodyMars.GetTrueAnomaly();

				if(marsAngle < earthAngle) marsAngle += 2.0 * Math.PI;  //always consider Mars to be ahead of Earth

				//We're looking for HeadstartRequired = CurrentHeadstart, but we need to cope with floating-point
				// inaccuracies.  The best way to do this is to look for a sign change in (HeadstartRequired - CurrentHeadstart)
				double planetAngularSeparation = marsAngle - earthAngle;
				if(satEventWatcher.SignChangeInDifference(marsHeadstartRequired, planetAngularSeparation))
				{
					showingSatellite = true;
					SatMajorAxisAngle = bodyEarth.GetTrueAnomaly();
					bodySatellite.Init();
					satOrbitRect = bodySatellite.OrbitBoundingRectUnrotated;
					satEventWatcher.Reset(); //now we will use the watcher to look for the satellite's arrival on Mars
					MoveToNextStatus(ShowInfoText, true, false);
					return;
				}
			}			
		}

		public void MoveToNextStatus(ShowInfoTextDelegate ShowInfoText, bool showContinueButton, bool showResetButton)
		{
			ResourceManager rm = new ResourceManager("Hohmann.InfoText", Assembly.GetExecutingAssembly());

			currStatus++;

			string infoMessage = rm.GetString("txtInfo" + currStatus.ToString(), new CultureInfo(System.String.Empty));					

			ShowInfoText(infoMessage, showContinueButton, showResetButton);
		}

		public void ChangeEarthRadius(int sliderValue)
		{
			bodyEarth.SemiMajorAxis = 1.0 + (sliderValue - 5) / 10.0;
			bodySatellite.SemiMajorAxis = (bodyEarth.SemiMajorAxis + bodyMars.SemiMajorAxis) / 2;
			bodySatellite.C = (bodyMars.SemiMajorAxis - bodyEarth.SemiMajorAxis) / 2;
			satOrbitRect = bodySatellite.OrbitBoundingRectUnrotated;
			satEventWatcher.Reset();
		}
	}

	/// <summary>
	/// Watches the sign of the difference between two doubles over successive calls and
	/// flags any changes.
	/// </summary>
	public class SignWatcher
	{
		private sbyte previousSign;

		public SignWatcher(double val1, double val2)
		{	previousSign = (val1 < val2) ? (sbyte) -1 : (sbyte) 1; }

		public SignWatcher()
		{	previousSign = 0; }

		public void Reset() { previousSign = 0; }

		public bool SignChangeInDifference(double val1, double val2)
		{	
			sbyte newSign = (val1 < val2) ? (sbyte) -1 : (sbyte) 1;
			bool blnSignChange = ((previousSign != 0) && (newSign != previousSign));

			previousSign = newSign;
			return blnSignChange;
		}
	}
}
