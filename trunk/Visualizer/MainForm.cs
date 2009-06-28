using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ifpfc.Logic;
using ifpfc.Logic.EccentricMeetAndGreet;
using ifpfc.Logic.Hohmann;
using ifpfc.Logic.MeetAndGreet;
using log4net.Config;

namespace Visualizer
{
	internal partial class MainForm : Form
	{
		private IController currentController_;
		private bool nowRunning_;
		private readonly CanvasPanel pnlCanvas;
		private readonly Timer renderTimer = new Timer { Interval = 100 };

		public MainForm()
		{
			InitializeComponent();

			splitContainer1.SplitterDistance = splitContainer1.Panel1.Height;

			pnlCanvas = new CanvasPanel { Parent = splitContainer1.Panel1 };

			SetUpSolversCombobox();

			XmlConfigurator.Configure();

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
				new KeyValuePair<string, ProblemDescription>(
					"MeetAndGreet - 1", new ProblemDescription(@"..\..\..\ProblemVMImages\bin2.obf", new MeetAndGreetSolver(), 2001)
				),
				new KeyValuePair<string, ProblemDescription>(
					"MeetAndGreet - 2", new ProblemDescription(@"..\..\..\ProblemVMImages\bin2.obf", new MeetAndGreetSolver(), 2002)
				),
				new KeyValuePair<string, ProblemDescription>(
					"MeetAndGreet - 3", new ProblemDescription(@"..\..\..\ProblemVMImages\bin2.obf", new MeetAndGreetSolver(), 2003)
				),
				new KeyValuePair<string, ProblemDescription>(
					"MeetAndGreet - 4", new ProblemDescription(@"..\..\..\ProblemVMImages\bin2.obf", new MeetAndGreetSolver(), 2004)
				),
				new KeyValuePair<string, ProblemDescription>(
					"EccentricMeetAndGreet - 1", new ProblemDescription(@"..\..\..\ProblemVMImages\bin3.obf", new EccentricMeetAndGreetSolver(3001), 3001)
				),
				new KeyValuePair<string, ProblemDescription>(
					"EccentricMeetAndGreet - 2", new ProblemDescription(@"..\..\..\ProblemVMImages\bin3.obf", new EccentricMeetAndGreetSolver(3002), 3002)
				),
				new KeyValuePair<string, ProblemDescription>(
					"EccentricMeetAndGreet - 3", new ProblemDescription(@"..\..\..\ProblemVMImages\bin3.obf", new EccentricMeetAndGreetSolver(3003), 3003)
				),
				new KeyValuePair<string, ProblemDescription>(
					"EccentricMeetAndGreet - 4", new ProblemDescription(@"..\..\..\ProblemVMImages\bin3.obf", new EccentricMeetAndGreetSolver(3004), 3004)
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
				pnlCanvas.ClearTrace();
				currentController_ = new Controller(problem);
				SetRunningState(true);
			};

			ShowCurrentSpeed();
		}

		private void SetRunningState(bool nowRunning)
		{
			nowRunning_ = nowRunning;
			btnRun_.Text = nowRunning_ ? "Pause" : "Run";
			btnRun_.Enabled = true;
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
			var state = currentController_.CurrentState;
			toolStripStatusLabel1.Text = string.Format(
				"Московское время {1} тиков, просимулировано {0} тиков; очки = {2}",
				currentController_.TicksSimulated,
				currentController_.CurrentTime,
				(state != null) ? state.Score.ToString() : "N/A");
			pnlCanvas.Render(state);
		}

		private int step = 300;
	}
}
