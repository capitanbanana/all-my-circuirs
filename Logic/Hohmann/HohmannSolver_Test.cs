using System.IO;
using log4net.Config;
using Xunit;

namespace ifpfc.Logic.Hohmann
{
	public class HohmannSolver_Test
	{
		[Fact]
		public void Fly1001()
		{
			XmlConfigurator.Configure();
			//var vm = new VirtualMachine(117, 1001, 1001, TestData.Hohmann);
			var vm = new HohmannsEngine(117, 1001, 1001);
			var driver = new Driver(new HohmannSolver());
			Vector dv = new Vector(0, 0);
			while (!driver.IsEnd())
			{
				var outPorts = vm.RunTimeStep(new Vector(dv.x, dv.y));
				dv = driver.RunStep(outPorts);
			}
			File.WriteAllBytes("res.bin", vm.CreateSubmission());
		}
	}
}