using System.IO;
using ifpfc.VM;
using log4net;
using log4net.Config;
using Xunit;

namespace ifpfc.Logic.EccentricMeetAndGreet
{
	public class EccentricMeetAndGreetSolver_Test
	{
		[Fact]
		public void Fly3001()
		{
			XmlConfigurator.Configure();
			//var vm = new VirtualMachine(117, 1001, 1001, TestData.Hohmann);
			const int scenarioId = 3001;
			var vm = new EccentricMeetAndGreetEngine(117, scenarioId);
			var driver = new Driver(new EccentricMeetAndGreetSolver(scenarioId));
			var dv = new Vector(0, 0);
			while (!driver.IsEnd())
			{
				var outPorts = vm.RunTimeStep(new Vector(dv.x, dv.y));
				dv = driver.RunStep(outPorts);
				//log.Info("TIME = " + time++);
				//if(vm.Mem[212] > 0) log.Info("GOOD_TIME = " + vm.Mem[212]);
				//log.Info("DISTANCE_TO_TARGET_ORBIT = " + vm.Mem[155]);
			}
			File.WriteAllBytes("res.bin", vm.CreateSubmission());
		}

		private static ILog log = LogManager.GetLogger(typeof(EccentricMeetAndGreetSolver_Test));
	}
}