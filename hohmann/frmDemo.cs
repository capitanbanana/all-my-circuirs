////////////////////////////////////////////////////////////////////
/// frmDemo.cs
/// © 2005 Carl Johansen
/// 
/// UI for Hohmann Transfer Orbit demonstration
////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace CarlInc.Demos.Hohmann
{
	/// <summary>
	/// UI for Hohmann Transfer Orbit demonstration
	/// </summary>
	public class frmDemo : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Timer tmrHeartbeat;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem mnuHelpAbout;
		private System.Windows.Forms.MenuItem mnuFileExit;

		private DemoController theDemo;
		private DemoController.ShowInfoTextDelegate showInfoTextDelegate;

		private readonly Color COLOUR_EARTH, COLOUR_MARS, COLOUR_SAT, COLOUR_SUN;

		private System.Windows.Forms.Label lblInfo; 
		private bool inInfoDisplayMode;
		private System.Windows.Forms.TrackBar trackEarthSize;
		private System.Windows.Forms.Button cmdContinue;
		private System.Windows.Forms.Button cmdReset;
		private System.Windows.Forms.Label lblDate;
		private int au2PixelScale;

		public frmDemo()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			theDemo = new DemoController();
			showInfoTextDelegate = new DemoController.ShowInfoTextDelegate(ShowInfoText);

			SetAU2PixelScale();
			COLOUR_EARTH = Color.CornflowerBlue;
			COLOUR_MARS = Color.RosyBrown;
			COLOUR_SAT = Color.BlanchedAlmond;
			COLOUR_SUN = Color.Goldenrod;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.tmrHeartbeat = new System.Windows.Forms.Timer(this.components);
			this.lblInfo = new System.Windows.Forms.Label();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.mnuFileExit = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.mnuHelpAbout = new System.Windows.Forms.MenuItem();
			this.trackEarthSize = new System.Windows.Forms.TrackBar();
			this.cmdContinue = new System.Windows.Forms.Button();
			this.cmdReset = new System.Windows.Forms.Button();
			this.lblDate = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.trackEarthSize)).BeginInit();
			this.SuspendLayout();
			// 
			// tmrHeartbeat
			// 
			this.tmrHeartbeat.Enabled = true;
			this.tmrHeartbeat.Interval = 10;
			this.tmrHeartbeat.Tick += new System.EventHandler(this.AdvanceOneDay);
			// 
			// lblInfo
			// 
			this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblInfo.BackColor = System.Drawing.Color.Black;
			this.lblInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblInfo.ForeColor = System.Drawing.Color.White;
			this.lblInfo.Location = new System.Drawing.Point(520, 64);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Size = new System.Drawing.Size(104, 64);
			this.lblInfo.TabIndex = 0;
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1,
																					  this.menuItem2});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnuFileExit});
			this.menuItem1.Text = "&File";
			// 
			// mnuFileExit
			// 
			this.mnuFileExit.Index = 0;
			this.mnuFileExit.Text = "E&xit";
			this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnuHelpAbout});
			this.menuItem2.Text = "&Help";
			// 
			// mnuHelpAbout
			// 
			this.mnuHelpAbout.Index = 0;
			this.mnuHelpAbout.Text = "&About...";
			this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
			// 
			// trackEarthSize
			// 
			this.trackEarthSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.trackEarthSize.Location = new System.Drawing.Point(520, 8);
			this.trackEarthSize.Name = "trackEarthSize";
			this.trackEarthSize.Size = new System.Drawing.Size(104, 45);
			this.trackEarthSize.TabIndex = 1;
			this.trackEarthSize.Value = 5;
			this.trackEarthSize.Scroll += new System.EventHandler(this.trackEarthSize_Scroll);
			// 
			// cmdContinue
			// 
			this.cmdContinue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdContinue.BackColor = System.Drawing.SystemColors.Control;
			this.cmdContinue.Location = new System.Drawing.Point(520, 168);
			this.cmdContinue.Name = "cmdContinue";
			this.cmdContinue.Size = new System.Drawing.Size(104, 24);
			this.cmdContinue.TabIndex = 2;
			this.cmdContinue.Text = "Continue";
			this.cmdContinue.Visible = false;
			this.cmdContinue.Click += new System.EventHandler(this.cmdContinue_Click);
			// 
			// cmdReset
			// 
			this.cmdReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdReset.BackColor = System.Drawing.SystemColors.Control;
			this.cmdReset.Location = new System.Drawing.Point(520, 200);
			this.cmdReset.Name = "cmdReset";
			this.cmdReset.Size = new System.Drawing.Size(104, 24);
			this.cmdReset.TabIndex = 3;
			this.cmdReset.Text = "Reset";
			this.cmdReset.Visible = false;
			this.cmdReset.Click += new System.EventHandler(this.cmdReset_Click);
			// 
			// lblDate
			// 
			this.lblDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblDate.BackColor = System.Drawing.Color.Black;
			this.lblDate.ForeColor = System.Drawing.Color.White;
			this.lblDate.Location = new System.Drawing.Point(520, 136);
			this.lblDate.Name = "lblDate";
			this.lblDate.Size = new System.Drawing.Size(104, 18);
			this.lblDate.TabIndex = 4;
			// 
			// frmDemo
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(632, 444);
			this.Controls.Add(this.lblDate);
			this.Controls.Add(this.cmdReset);
			this.Controls.Add(this.cmdContinue);
			this.Controls.Add(this.trackEarthSize);
			this.Controls.Add(this.lblInfo);
			this.Menu = this.mainMenu1;
			this.Name = "frmDemo";
			this.Text = "Hohmann Transfer Orbit Demo";
			this.Resize += new System.EventHandler(this.frmDemo_Resize);
			this.Load += new System.EventHandler(this.frmDemo_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmDemo_Paint);
			((System.ComponentModel.ISupportInitialize)(this.trackEarthSize)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.Run(new frmDemo());
		}

		private int AU2Pixels(double x) 
		{   // convert Astronomical Units (the unit of measure of the OrbitingBody classes) to pixels
			return (int) (x * au2PixelScale); 
		}

		private void SetAU2PixelScale()
		{
			au2PixelScale = (int) (this.ClientRectangle.Height / theDemo.DisplayHeightInAU()) - 20;
		}

		private void frmDemo_Load(object sender, System.EventArgs e)
		{
			//use double-buffering for performance (you've got plenty of memory, right?)
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);

			StartDemo();
		}

		private void StartDemo()
		{
			theDemo.Init();
			inInfoDisplayMode = false;			
			cmdReset.Visible = false;
			trackEarthSize.Value = 5;
			theDemo.AdvanceOneDay(showInfoTextDelegate);
			lblDate.Text = theDemo.SimulationCurrDate.ToString("dd-MMM-yyyy");
			theDemo.MoveToNextStatus(showInfoTextDelegate, true, false);
		}

		private void frmDemo_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			const double RAD_TO_DEG = 180.0F / Math.PI;

			Graphics formGraphics = e.Graphics;
			Pen orbitPen = new System.Drawing.Pen(Color.White, 1);
			SolidBrush bodyBrush = new SolidBrush(Color.White);
			formGraphics.TranslateTransform(this.ClientRectangle.Width / 2, this.ClientRectangle.Height / 2);

			//draw Earth orbit (circular)
			int r = AU2Pixels(theDemo.bodyEarth.SemiMajorAxis);
			orbitPen.Color = COLOUR_EARTH;
			formGraphics.DrawEllipse(orbitPen, new Rectangle(-r, -r, 2*r, 2*r));

			//draw Mars orbit (circular)
			r = AU2Pixels(theDemo.bodyMars.SemiMajorAxis);
			orbitPen.Color = COLOUR_MARS;
			formGraphics.DrawEllipse(orbitPen, new Rectangle(-r, -r, 2*r, 2*r));

			//draw Sun
			bodyBrush.Color = COLOUR_SUN;
			formGraphics.FillEllipse(bodyBrush, new Rectangle(-10, -10, 20, 20));

			PointF currEarthPosAU = theDemo.bodyEarth.GetCurrXY(), currMarsPosAU = theDemo.bodyMars.GetCurrXY();
			Point currEarthPos = new Point(	AU2Pixels(currEarthPosAU.X), AU2Pixels(currEarthPosAU.Y) );
			Point currMarsPos = new Point( AU2Pixels(currMarsPosAU.X), AU2Pixels(currMarsPosAU.Y) );

			//draw Earth
			bodyBrush = new SolidBrush(COLOUR_EARTH);
			formGraphics.FillEllipse(bodyBrush, new	Rectangle(currEarthPos.X - 5, currEarthPos.Y - 5, 10, 10));

			//draw Mars
			bodyBrush.Color = COLOUR_MARS;
			formGraphics.FillEllipse(bodyBrush, new	Rectangle(currMarsPos.X - 4, currMarsPos.Y - 4, 8, 8));

			if(theDemo.ShowingSatellite)
			{
				PointF currSatPosAU = theDemo.bodySatellite.GetCurrXY();
				Point currSatPos = new Point(AU2Pixels(currSatPosAU.X), AU2Pixels(currSatPosAU.Y) );

				// Rotate the system to the Earth-Sun angle (have to convert angle to degrees)
				formGraphics.RotateTransform(-1 * (float) (theDemo.SatMajorAxisAngle * RAD_TO_DEG), System.Drawing.Drawing2D.MatrixOrder.Prepend );

				//draw Satellite orbit
				orbitPen.Color = COLOUR_SAT;
				formGraphics.DrawEllipse(orbitPen, new Rectangle(AU2Pixels(theDemo.SatOrbitRectangle.Left), AU2Pixels(theDemo.SatOrbitRectangle.Top), AU2Pixels(theDemo.SatOrbitRectangle.Width), AU2Pixels(theDemo.SatOrbitRectangle.Height)));

				//draw Satellite
				bodyBrush.Color = COLOUR_SAT;
				formGraphics.FillEllipse(bodyBrush, new	Rectangle(currSatPos.X - 3, currSatPos.Y - 3, 6, 6));
			}

			bodyBrush.Dispose();
			orbitPen.Dispose();
		}

		private void AdvanceOneDay(object sender, System.EventArgs e)
		{	
			theDemo.AdvanceOneDay(showInfoTextDelegate);
			lblDate.Text = theDemo.SimulationCurrDate.ToString("dd-MMM-yyyy");
			this.Invalidate();
		}

		public void ShowInfoText(string infoMessage, bool showContinueButton, bool showResetButton)
		{
			//display the specified text in a label and (optionally) pause the simulation
			lblInfo.Text = infoMessage;
			inInfoDisplayMode = true;

			tmrHeartbeat.Enabled = !showContinueButton;
			cmdContinue.Visible = showContinueButton;
			cmdReset.Visible = showResetButton;			
		}

		private void mnuHelpAbout_Click(object sender, System.EventArgs e)
		{
			MessageBox.Show(this, "Hohmann Transfer Orbit demonstration version " + Application.ProductVersion.ToString() + "\n© 2005 Carl Johansen",
				"About this application", 
				MessageBoxButtons.OK,
				MessageBoxIcon.Information);
		}

		private void mnuFileExit_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void frmDemo_Resize(object sender, System.EventArgs e)
		{
			SetAU2PixelScale();
			this.Invalidate();
		}

		private void trackEarthSize_Scroll(object sender, System.EventArgs e)
		{
			theDemo.ChangeEarthRadius(trackEarthSize.Value);
			this.Invalidate();
		}

		private void cmdContinue_Click(object sender, System.EventArgs e)
		{
			if(inInfoDisplayMode)
			{
				//hide the info text display and resume the simulation
				inInfoDisplayMode = false;
				trackEarthSize.Enabled = true;
				cmdContinue.Visible = false;
				tmrHeartbeat.Enabled = true;
				theDemo.MoveToNextStatus(showInfoTextDelegate, false, cmdReset.Visible);
			}				
		}

		private void cmdReset_Click(object sender, System.EventArgs e)
		{
			this.Invalidate();
			StartDemo();
		}

	}
}