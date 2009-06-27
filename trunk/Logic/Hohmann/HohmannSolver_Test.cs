using ifpfc.VM;
using SKBKontur.LIT.Core;
using Xunit;

namespace ifpfc.Logic.Hohmann
{
	public class HohmannSolver_Test
	{
		private static Directory VMImagesDirectory
		{
			get
			{
				Directory result = Directory.CurrentDirectory;
				while (result.GetFiles("*.csproj").Length == 0)
					result = result.Parent;
				return result.GetDirectory("ProblemVMImages");
			}
		}

		[Fact]
		public void Fly1001()
		{
			var vm = new VirtualMachine(42, 1001, 1001, ReadImage("bin1.obf"));
			var driver = new Driver<HohmannState>(new HohmannSolver());
			Vector dv = new Vector(0, 0);
			while (!driver.IsEnd())
			{
				var outPorts = vm.RunTimeStep(new Vector(dv.x, dv.y));
				dv = driver.RunStep(outPorts);
			}
		}

		private static byte[] ReadImage(string imageName)
		{
			return VMImagesDirectory.GetFile(imageName).Content.Data;
		}
	}
}