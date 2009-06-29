using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ifpfc;
using ifpfc.Logic;

namespace Visualizer
{
	[System.ComponentModel.DesignerCategory("NotDesignable")]
	internal class CanvasPanel : Panel
	{
		private static readonly Color EarthColor = Color.LightBlue;
		private static readonly Color VoyagerColor = Color.Red;
		private static readonly Color TargetColor = Color.LightGreen;

		private readonly Font defaultFont_ = new Font(SystemFonts.DefaultFont.FontFamily, SystemFonts.DefaultFont.Size);
		private readonly StringFormat textFormat_;
		private readonly Pen orbitPen_ = new Pen(Color.White, 1);
		private readonly SolidBrush bodyBrush_ = new SolidBrush(Color.White);
		private VisualizerState dataSource_;

		private readonly HashSet<Point> tracePixels = new HashSet<Point>();
		private readonly List<Point> trace = new List<Point>();
		private bool rememberTrace;

		public CanvasPanel()
		{
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

		public void Render(VisualizerState newState)
		{
			dataSource_ = newState;
			Invalidate();
		}

		public void ClearTrace()
		{
			trace.Clear();
			tracePixels.Clear();
			rememberTrace = true;
		}

		private int ScaleDistance(double x)
		{
			return (int)(x * ClientRectangle.Height / dataSource_.UniverseDiameter);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (dataSource_ == null)
				return;

			Graphics gr = e.Graphics;
			gr.TranslateTransform(ClientRectangle.Width / 2.0f, ClientRectangle.Height / 2.0f);

			bodyBrush_.Color = EarthColor;
			int scaledR = ScaleDistance(Physics.R);
			gr.FillEllipse(bodyBrush_, new Rectangle(-scaledR, -scaledR, 2*scaledR, 2*scaledR));
			bodyBrush_.Color = VoyagerColor;
			DrawCaption(gr, "Земля в иллюминаторе", -scaledR, 3);

			DrawSattelite(gr, dataSource_.Voyager, VoyagerColor, false);
			foreach (var t in dataSource_.Targets)
				DrawSattelite(gr, t, TargetColor, true);
			foreach (var orbit in dataSource_.FixedOrbits)
				DrawOrbit(gr, orbit, Color.Yellow);
			foreach (var tracePixel in trace)
				gr.DrawEllipse(Pens.LightGreen, tracePixel.X, tracePixel.Y, 1, 1);
		}

		private void DrawCaption(Graphics gr, string caption, int x, int y)
		{
			var size = gr.MeasureString(caption, defaultFont_, 0, textFormat_);
			var titleRect = new RectangleF(x, y, size.Width, size.Height);
			gr.DrawString(caption, defaultFont_, bodyBrush_, titleRect, textFormat_);
		}

		private void DrawSattelite(Graphics gr, Sattelite s, Color color, bool updateTrace)
		{
			var pos = new Point(ScaleDistance(s.Location.x), ScaleDistance(s.Location.y));
			bodyBrush_.Color = color;
			gr.FillEllipse(bodyBrush_, new Rectangle(pos.X - 3, pos.Y - 3, 6, 6));
			DrawCaption(gr, s.Name, pos.X + 2, pos.Y + 2);

			if (rememberTrace && updateTrace)
			{
				if (tracePixels.Contains(pos))
					rememberTrace = false;
				else
				{
					tracePixels.Add(pos);
					trace.Add(pos);
				}
			}
		}

		private void DrawOrbit(Graphics gr, Orbit orbit, Color color)
		{
			try
			{
				if (orbit.SemiMajorAxis > 0)
				{
					var gs = gr.Save();
					var transformAngle = (float)(-1 * (orbit.TransformAngle + Math.PI/2) * (180.0F / Math.PI));
					gr.RotateTransform(transformAngle, MatrixOrder.Prepend);

					orbitPen_.Color = color;
					var or = new RectangleF(
						(float)(-1 * orbit.SemiMinorAxis), (float)(-1 * orbit.PeriapsisDistance),
						(float)(2 * orbit.SemiMinorAxis), (float)(2 * orbit.SemiMajorAxis)
						);
					gr.DrawEllipse(
						orbitPen_,
						new Rectangle(ScaleDistance(or.Left), ScaleDistance(or.Top), ScaleDistance(or.Width), ScaleDistance(or.Height))
						);
					gr.Restore(gs);
				}
			}
			catch(OverflowException)
			{
			}
		}
	}
}