using System;
using System.IO;
using ifpfc.VM;
using log4net;
using log4net.Config;
using Xunit;

namespace ifpfc.Logic.MeetAndGreet
{
	public class MeetAndGreetSolver_Test
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (MeetAndGreetSolver_Test));

		[Fact]
		public void Fly200x()
		{
			XmlConfigurator.Configure();
			var vm = new MeetAndGreetEngine(117, 2003, 2003);
			var driver = new Driver(new MeetAndGreetSolver());
			var dv = new Vector(0, 0);
			int goodTime = 0;
			bool repeated = false;
			while (!driver.IsEnd())
			{
				double[] outPorts = vm.RunTimeStep(new Vector(dv.x, dv.y));
				dv = driver.RunStep(outPorts);
				//log.Info("TIME = " + time++);
				if (vm.Mem[336] > 0)
				{
					goodTime = (int) vm.Mem[336];
					log.Info("GOOD_TIME = " + goodTime);
				}
				else if (goodTime > 0)
				{
					if (repeated) break;
					repeated = true;
				}
				else
				{
					repeated = false;
				}
				//log.Info("DISTANCE_TO_TARGET_ORBIT = " + vm.Mem[155]);
			}
			if (driver.UnderlyingSolver.State.Score > 0)
			{
				File.WriteAllBytes("res2001.bin", vm.CreateSubmission());
				Console.WriteLine(driver.UnderlyingSolver.State.Score);
			}
		}
	}
}