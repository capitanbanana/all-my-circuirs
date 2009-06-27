using log4net.Config;
using ifpfc.VM;
using SKBKontur.LIT.Core;
using Xunit;

namespace ifpfc.Logic.Hohmann
{
	public class HohmannSolver_Test
	{
		[Fact]
		public void Fly1001()
		{
			XmlConfigurator.Configure();
			var vm = new VirtualMachine(42, 1001, 1001, TestData.Hohmann);
			var driver = new Driver(new HohmannSolver());
			Vector dv = new Vector(0, 0);
			while (!driver.IsEnd())
			{
				var outPorts = vm.RunTimeStep(new Vector(dv.x, dv.y));
				dv = driver.RunStep(outPorts);
			}
		}
	}
}