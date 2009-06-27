using SKBKontur.LIT.Core;
using Xunit;

namespace ifpfc
{
	public class VirtualMachine_Test
	{
		[Fact]
		public void can_run_first_image()
		{
			var vm = new VirtualMachine(42, 42, 42, ReadImage("bin1.obf"));
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