using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Visualizer
{
	[System.ComponentModel.DesignerCategory("NotDesignable")]
	internal class CanvasPanel : Panel
	{
		private const double RadToDeg = 180.0F / Math.PI;

		private static readonly Color EarthColor = Color.Blue;
		private static readonly Color VoyagerColor = Color.Red;
		private static readonly Color TargetColor = Color.Green;

		private readonly ISystemState dataSource_;

		private readonly StringFormat textFormat_;
		private readonly Pen orbitPen_ = new Pen(Color.White, 1);
		private readonly SolidBrush bodyBrush_ = new SolidBrush(Color.White);

		public CanvasPanel(ISystemState dataSource)
		{
			if (dataSource == null) throw new ArgumentNullException("dataSource");
			dataSource_ = dataSource;

			base.AutoSize = true;
			base.Dock = DockStyle.Fill;
			base.BackColor = Color.Black;

			textFormat_ = StringFormat.GenericDefault;
			textFormat_.LineAlignment = StringAlignment.Near;
			textFormat_.Alignment = StringAlignment.Near;
			textFormat_.Trimming = StringTrimming.EllipsisCharacter;

			Margin = Padding.Empty;
			Padding = Padding.Empty;

			//reset control style to improve rendering (reduce flicker)
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.UserPaint, true);
		}

		private int Au2Pixels(double x)
		{   // convert Astronomical Units (the unit of measure of the OrbitingBody classes) to pixels
			return (int)(x * ClientRectangle.Height / 1000.0);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			Graphics gr = e.Graphics;
			ResetTransform(gr);

			//earth
			bodyBrush_.Color = EarthColor;
			gr.FillEllipse(bodyBrush_, new Rectangle(-5, -5, 10, 10));

			var titleRect = new Rectangle(11, 11, 50, 15);
			e.Graphics.DrawString("Earth", SystemFonts.DefaultFont, Brushes.White, titleRect, textFormat_);

			DrawSattelite(gr, dataSource_.Voyager, VoyagerColor);
			foreach(var t in dataSource_.Targets)
				DrawSattelite(gr, t, TargetColor);
		}

		private void ResetTransform(Graphics gr)
		{
			gr.ResetTransform();
			gr.TranslateTransform(ClientRectangle.Width / 2.0f, ClientRectangle.Height / 2.0f);
			var mxFlipY = new Matrix(1, 0, 0, -1, 0, ClientRectangle.Height);
            gr.MultiplyTransform(mxFlipY, MatrixOrder.Append);
		}

		private void DrawSattelite(Graphics gr, Sattelite s, Color color)
		{
			var posAu = new PointF((float)s.Location.X, (float)s.Location.Y);
			var pos = new Point(Au2Pixels(posAu.X), Au2Pixels(posAu.Y));
			bodyBrush_.Color = color;
			gr.FillEllipse(bodyBrush_, new Rectangle(pos.X - 3, pos.Y - 3, 6, 6));

			// Rotate the system to the Earth-Sun angle (have to convert angle to degrees)
			var transformAngle = (float)(-1 * s.Orbit.TransformAngle * RadToDeg);
			gr.RotateTransform(transformAngle, MatrixOrder.Prepend);

			orbitPen_.Color = color;
            var or = new RectangleF(
				(float)(-1 * s.Orbit.SemiMinorAxis), (float)(-1 * s.Orbit.PeriapsisDistance),
				(float)(2 * s.Orbit.SemiMinorAxis), (float)(2 * s.Orbit.SemiMajorAxis)
			);
			gr.DrawEllipse(orbitPen_, new Rectangle(Au2Pixels(or.Left), Au2Pixels(or.Top), Au2Pixels(or.Width), Au2Pixels(or.Height)));

			ResetTransform(gr);
		}
	}
}