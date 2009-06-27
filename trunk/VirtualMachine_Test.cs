using log4net.Config;
using SKBKontur.LIT.Core;
using Xunit;

namespace ifpfc
{
	public class VirtualMachine_Test
	{
		public VirtualMachine_Test()
		{
			XmlConfigurator.Configure();
		}

		[Fact]
		public void can_run_first_image()
		{
			var vm = new VirtualMachine(42, 42, 42, ReadImage("bin1.obf"));
			vm.RunTimeStep(0.0, 0.0);
		}

		[Fact]
		public void can_run_second_image()
		{
			var vm = new VirtualMachine(42, 42, 42, ReadImage("bin2.obf"));
			vm.RunTimeStep(0.0, 0.0);
		}

		[Fact]
		public void can_run_third_image()
		{
			var vm = new VirtualMachine(42, 42, 42, ReadImage("bin3.obf"));
			vm.RunTimeStep(0.0, 0.0);
		}

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

		private static byte[] ReadImage(string imageName)
		{
			return VMImagesDirectory.GetFile(imageName).Content.Data;
		}
	}
}