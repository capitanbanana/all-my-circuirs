using System.Drawing;
using System.Windows.Forms;

namespace Visualizer
{
	[System.ComponentModel.DesignerCategory("NotDesignable")]
	internal class CanvasPanel : Panel
	{
		protected readonly StringFormat textFormat_;

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

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			Rectangle headerRect = ClientRectangle;
			ControlPaint.DrawBorder3D(e.Graphics, headerRect, Border3DStyle.Etched, Border3DSide.Bottom);

			e.Graphics.FillRectangle(SystemBrushes.Desktop, headerRect);

			headerRect.Height -= SystemInformation.Border3DSize.Height;

			var textFormat = textFormat_;
			var titleRect = new Rectangle(10, 10, 100, 100);
			e.Graphics.DrawString("Hello world", SystemFonts.DefaultFont, SystemBrushes.WindowText, titleRect, textFormat);

		}
	}
}