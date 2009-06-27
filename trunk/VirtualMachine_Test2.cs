using System;
using System.Collections;
using System.IO;
using Xunit;

namespace ifpfc
{
	public class VirtualMachine_Test2
	{
		private MemoryStream image = new MemoryStream();

		[Fact]
		public void Output()
		{
			WriteD(5, 0, 0, 3.14);
			WriteD(5, 1, 0, 123);
			WriteD(5, 2, 2, 321);
			WriteD(5, 3, 0, 321);
			WriteD(5, 3, 4, 4.4);
			var output = Run();
			Assert.Equal(3.14, output[0]);
			Assert.Equal(3.14, output[1]);
			Assert.Equal(321, output[2]);
		}

		[Fact]
		public void Add()
		{
			WriteD(1, 0, 1, 2);
			WriteD(5, 0, 0, 3);
			var output = Run();
			Assert.Equal(5.0, output[0]);
		}
	
		[Fact]
		public void Sub()
		{
			WriteD(2, 0, 1, 2);
			WriteD(5, 0, 0, 3);
			var output = Run();
			Assert.Equal(-1.0, output[0]);
		}

		[Fact]
		public void Mult()
		{
			WriteD(3, 0, 1, 2);
			WriteD(5, 0, 0, 3);
			var output = Run();
			Assert.Equal(6.0, output[0]);
		}		
		
		[Fact]
		public void Div()
		{
			WriteD(4, 0, 1, 3);
			WriteD(5, 0, 0, 2);
			WriteD(5, 1, 2, 666);
			WriteD(4, 3, 4, 3);
			WriteD(5, 1, 3, 0);
			var output = Run();
			Assert.Equal(1.5, output[0]);
			Assert.Equal(0.0, output[1]);
		}		
		
		[Fact]
		public void Noop()
		{
			WriteS(0, 0, 1);
			WriteS(0, 0, 2);
			WriteD(5, 0, 0, 0);
			WriteD(5, 1, 1, 0);
			var output = Run();
			Assert.Equal(1.0, output[0]);
			Assert.Equal(2.0, output[1]);
			Assert.Equal(0.0, output[2]);
		}

		[Fact]
		public void Less()
		{
			CheckComparison(0, -1, true);
			CheckComparison(0, 100, false);
			CheckComparison(0, 0, false);
			CheckComparison(0, -0.0000001, true);
			CheckComparison(0, 0.0000001, false);
			CheckComparison(0, 1e10, false);
			CheckComparison(0, 1e-10, false);
			CheckComparison(0, -1e-10, true);
			CheckComparison(0, -1e10, true);
		}

		[Fact]
		public void LessOrEqual()
		{
			CheckComparison(1, 0, true);
			CheckComparison(1, -1, true);
			CheckComparison(1, 1, false);
			CheckComparison(1, -0.0000001, true);
			CheckComparison(1, 0.0000001, false);
		}

		[Fact]
		public void Equal()
		{
			CheckComparison(2, -1, false);
			CheckComparison(2, 1, false);
			CheckComparison(2, 0, true);
			CheckComparison(2, -0.0000001, false);
			CheckComparison(2, 0.0000001, false);
		}

		[Fact]
		public void GreaterOrEqual()
		{
			CheckComparison(3, -1, false);
			CheckComparison(3, 1, true);
			CheckComparison(3, 0, true);
			CheckComparison(3, -0.0000001, false);
			CheckComparison(3, 0.0000001, true);
		}

		[Fact]
		public void Greater()
		{
			CheckComparison(4, -1, false);
			CheckComparison(4, 1, true);
			CheckComparison(4, 0, false);
			CheckComparison(4, -0.0000001, false);
			CheckComparison(4, 0.0000001, true);
		}
	
		[Fact]
		public void Sqrt()
		{
			WriteS(2, 1, 25);
			WriteD(5, 0, 0, 16);
			WriteS(2, 3, 666);
			WriteD(5, 1, 2, 0);
			var output = Run();
			Assert.Equal(4.0, output[0]);
			Assert.Equal(0.0, output[1]);
		}

		[Fact]
		public void Copy()
		{
			WriteS(3, 1, 25);
			WriteD(5, 0, 0, 3.14);
			WriteS(3, 2, 25);
			WriteD(5, 1, 2, 3.14);
			var output = Run();
			Assert.Equal(3.14, output[0]);
			Assert.Equal(25.0, output[1]);
		}
		
		[Fact]
		public void Input()
		{
			WriteS(4, 2, 0);
			WriteD(5, 0, 0, 0);
			WriteS(4, 3, 0);
			WriteD(5, 1, 2, 0);
			var output = Run(1, 2);
			Assert.Equal(1, output[0]);
			Assert.Equal(2, output[1]);
		}

		private void CheckComparison(int cmpCode, double value, bool isTrue)
		{
			image = new MemoryStream();
			WriteCmp(cmpCode, 0, value);	//	if (value <opcode> 0) 
			WriteD(6, 1, 2, 1);				//		mem[1] = 1 else mem[1] = 6
			WriteD(5, 0, 1, 6);				//	out[0] = mem[1]
			var output = Run();
			Assert.Equal(isTrue ? 1 : 6, output[0]);
		}

		private double[] Run()
		{
			return Run(0,0);
		}

		private double[] Run(double dx, double dy)
		{
			var output = new VirtualMachine(1, 1, 1, image.ToArray()).RunTimeStep(dx, dy);
			LogOutputPorts(output);
			return output;
		}

		private static void LogOutputPorts(double[] output)
		{
			for (int i = 0; i < 10; i++)
				Console.WriteLine("port {0} = {1}", i, output[i]);
		}

		private static string BitsToString(byte[] bytes)
		{
			var s = "";
			foreach (bool v in new BitArray(bytes))
				s += v ? "1" : "0";
			return s;
		}

		private void WriteD(byte op, int r1, int r2, double data)
		{
			var instructionsBytes = BitConverter.GetBytes(op << 28 | r1 << 14 | r2);
			var dataBytes = BitConverter.GetBytes(data);
			WriteFrame(dataBytes, instructionsBytes);
		}

		private void WriteS(byte op, int r1, double data)
		{
			var instructionsBytes = BitConverter.GetBytes(op << 24 | r1);
			var dataBytes = BitConverter.GetBytes(data);
			WriteFrame(dataBytes, instructionsBytes);
		}
		
		private void WriteCmp(int cmpCode, int r1, double data)
		{
			var instructionsBytes = BitConverter.GetBytes(1 << 24 | cmpCode << 21 | r1);
			var dataBytes = BitConverter.GetBytes(data);
			WriteFrame(dataBytes, instructionsBytes);
		}

		private void WriteFrame(byte[] dataBytes, byte[] instructionsBytes)
		{
			if ((image.Length/12)%2 == 0)
			{
				Log(dataBytes, instructionsBytes);
				image.Write(dataBytes, 0, dataBytes.Length);
				image.Write(instructionsBytes, 0, instructionsBytes.Length);
			}
			else
			{
				Log(instructionsBytes, dataBytes);
				image.Write(instructionsBytes, 0, instructionsBytes.Length);
				image.Write(dataBytes, 0, dataBytes.Length);
			}
		}

		private static void Log(byte[] b1, byte[] b2)
		{
			Console.WriteLine(BitsToString(b1) + " " + BitsToString(b2));
		}
	}
}