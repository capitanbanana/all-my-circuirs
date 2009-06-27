using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ifpfc;
using ifpfc.Logic.Hohmann;

namespace Visualizer
{
	internal partial class MainForm : Form, IVisualizer
	{
		private readonly VisualizerStateDataSource ds_ = new VisualizerStateDataSource();
		private IController currentController_;
		private bool nowRunning_;

		public MainForm()
		{
			InitializeComponent();

			splitContainer1.SplitterDistance = splitContainer1.Panel1.Height;

			var pnlCanvas = new CanvasPanel(ds_) { Parent = splitContainer1.Panel1 };

			SetUpSolversCombobox();
		}

		private void SetUpSolversCombobox()
		{
			cmbxSolvers_.DropDownStyle = ComboBoxStyle.DropDownList;

			var scenarios = new List<KeyValuePair<string, ProblemDescription>>(new[]{
				new KeyValuePair<string, ProblemDescription>("Select scenario", null),
				new KeyValuePair<string, ProblemDescription>(
					"Hohmann - 1", new ProblemDescription(@"..\..\..\ProblemVMImages\bin1.obf", new HohmannSolver(), 1)
				),
				new KeyValuePair<string, ProblemDescription>(
					"Hohmann - 2", new ProblemDescription(@"..\..\..\ProblemVMImages\bin1.obf", new HohmannSolver(), 2)
				),
				new KeyValuePair<string, ProblemDescription>(
					"Hohmann - 3", new ProblemDescription(@"..\..\..\ProblemVMImages\bin1.obf", new HohmannSolver(), 3)
				),
				new KeyValuePair<string, ProblemDescription>(
					"Hohmann - 4", new ProblemDescription(@"..\..\..\ProblemVMImages\bin1.obf", new HohmannSolver(), 4)
				),
			});
			cmbxSolvers_.ComboBox.DataSource = scenarios;
			cmbxSolvers_.ComboBox.DisplayMember = "Key";
			cmbxSolvers_.ComboBox.ValueMember = "Value";
			cmbxSolvers_.ComboBox.MaxDropDownItems = scenarios.Count;

			cmbxSolvers_.ComboBox.SelectedValueChanged += (sender, args) =>
			{
				var problem = (ProblemDescription)cmbxSolvers_.ComboBox.SelectedValue;
				if (problem == null) return;
				currentController_ = new Controller(problem, this);
				SetRunningState(false);
			};
		}

		private void SetRunningState(bool nowRunning)
		{
			nowRunning_ = nowRunning;
			btnRun_.Text = nowRunning_ ? "Pause" : "Run";
			btnForward_.Enabled = !nowRunning_;
			btnBackward_.Enabled = !nowRunning_;
		}

		private void btnRun__Click(object sender, EventArgs e)
		{
			if (currentController_ == null) return;
			if (nowRunning_)
			{
				currentController_.SetSimulationMode(SimulationMode.Manual);
				SetRunningState(false);
			}
			else
			{
				currentController_.SetSimulationMode(SimulationMode.Manual);
				SetRunningState(true);
			}
		}

		private void btnBackward__Click(object sender, EventArgs e)
		{
			if (currentController_ == null) return;
			currentController_.StepBackward();
		}

		private void btnForward__Click(object sender, EventArgs e)
		{
			if (currentController_ == null) return;
			currentController_.StepForward();
		}


		

		public void Render(VisualizerState state)
		{
			ds_.UpdateState(state);
			Invalidate();
		}
	}
}
