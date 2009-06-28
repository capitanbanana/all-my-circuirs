using System;
using ifpfc.Logic;
using log4net.Config;
using SKBKontur.LIT.Core;
using Xunit;

namespace ifpfc.VM
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
			var vm = new VirtualMachine(42, 3001, 3001, ReadImage("bin3.obf"));
			var ports = vm.RunTimeStep(Vector.Zero);
			LogOutputPorts(ports);
		}
		
		private static void LogOutputPorts(double[] output)
		{
			for (int i = 0; i < 10; i++)
				Console.WriteLine("port {0} = {1}", i, output[i]);
		}

		[Fact]
		public void can_run_second_image()
		{
			var vm = new VirtualMachine(42, 42, 42, ReadImage("bin2.obf"));
			vm.RunTimeStep(Vector.Zero);
		}

		[Fact]
		public void can_run_third_image()
		{
			var vm = new VirtualMachine(42, 42, 42, ReadImage("bin3.obf"));
			vm.RunTimeStep(Vector.Zero);
		}

		[Fact]
		public void create_test_submission()
		{
			var vm = new VirtualMachine(117, 1001, 1001, ReadImage("bin1.obf"));
			const int scorePort = 0;
			string fuelFile = VMImagesDirectory.GetFile("fuel.txt").FullPath;
			double[] outport = null;
			int ticks = 0;
			while ((ticks < 50) && ((outport == null) || (outport[scorePort] == 0.0)))
			{
				outport = vm.RunTimeStep(Vector.Zero);
				System.IO.File.WriteAllLines(
					fuelFile,
					new[]
						{
							"Score: " + outport[0],
							"Fuel: " + outport[1],
							"X to Earth: " + outport[2],
							"Y to Earth: " + outport[3],
							"Ticks: " + ticks,
						});
				ticks++;
			}
			VMImagesDirectory.GetFile("bin1.osf").Write(new BinaryData(vm.CreateSubmission()));
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