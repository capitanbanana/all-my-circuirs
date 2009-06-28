using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ifpfc;
using ifpfc.Logic.Hohmann;

namespace Visualizer
{
	internal partial class MainForm : Form
	{
		private readonly VisualizerState ds_;
		private IController currentController_;
		private bool nowRunning_;
		private readonly CanvasPanel pnlCanvas;
		private readonly Timer renderTimer = new Timer { Interval = 50 };

		public MainForm()
		{
			InitializeComponent();

			splitContainer1.SplitterDistance = splitContainer1.Panel1.Height;

			pnlCanvas = new CanvasPanel { Parent = splitContainer1.Panel1 };

			SetUpSolversCombobox();

			renderTimer.Tick += (sender, args) => Render();
			renderTimer.Start();
		}

		private void SetUpSolversCombobox()
		{
			cmbxSolvers_.DropDownStyle = ComboBoxStyle.DropDownList;

			var scenarios = new List<KeyValuePair<string, ProblemDescription>>(new[]{
				new KeyValuePair<string, ProblemDescription>("Select scenario", null),
				new KeyValuePair<string, ProblemDescription>(
					"Hohmann - 1", new ProblemDescription(@"..\..\..\ProblemVMImages\bin1.obf", new HohmannSolver(), 1001)
				),
				new KeyValuePair<string, ProblemDescription>(
					"Hohmann - 2", new ProblemDescription(@"..\..\..\ProblemVMImages\bin1.obf", new HohmannSolver(), 1002)
				),
				new KeyValuePair<string, ProblemDescription>(
					"Hohmann - 3", new ProblemDescription(@"..\..\..\ProblemVMImages\bin1.obf", new HohmannSolver(), 1003)
				),
				new KeyValuePair<string, ProblemDescription>(
					"Hohmann - 4", new ProblemDescription(@"..\..\..\ProblemVMImages\bin1.obf", new HohmannSolver(), 1004)
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
				currentController_ = new Controller(problem);
				SetRunningState(true);
			};

			ShowCurrentSpeed();
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
			SetRunningState(!nowRunning_);
		}

		private void btnBackward__Click(object sender, EventArgs e)
		{
			Step(-step);
		}

		private void btnForward__Click(object sender, EventArgs e)
		{
			Step(step);
		}

		private void setSpeedButton_Click(object sender, EventArgs e)
		{
			int.TryParse(tbSpeed.Text, out step);
			ShowCurrentSpeed();
		}

		private void Step(int stepSize)
		{
			if (currentController_ == null) return;
			currentController_.Step(stepSize);
		}

		private void ShowCurrentSpeed()
		{
			tbSpeed.Text = step.ToString();
		}

		public void Render()
		{
			if (currentController_ == null)
				return;
			if (nowRunning_)
				currentController_.Step(step);
			toolStripStatusLabel1.Text = string.Format(
				"Московское время {1} тиков, просимулировано {0} тиков",
				currentController_.TicksSimulated,
				currentController_.CurrentTime);
			pnlCanvas.Render(currentController_.CurrentState);
		}

		private int step = 100;
	}
}
