using System;
using Xunit;

namespace ifpfc.VM
{
	public class Disassembler_Test
	{
		[Fact]
		public void Disassemble()
		{
			string disassemble = new Disasembler(TestData.Hohmann).Disassemble();
			Console.WriteLine(disassemble);
		}
	}
}