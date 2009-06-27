using System.Windows.Forms;

namespace Visualizer
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();

			var pnlCanvas = new CanvasPanel { Parent = splitContainer1.Panel1 };
		}
	}
}
