using System.Windows.Forms;

namespace Visualizer
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();

			splitContainer1.SplitterDistance = splitContainer1.Panel1.Height;

			var pnlCanvas = new CanvasPanel(new DummySystemState()) { Parent = splitContainer1.Panel1 };
		}
	}
}
