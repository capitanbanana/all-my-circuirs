using System;
using Xunit;

namespace ifpfc.VM
{
	public class Disassembler_Test
	{
		[Fact]
		public void Disassemble()
		{
			string disassemble = new Disasembler(TestData.MeetAndGreet).Disassemble();
			Console.WriteLine(disassemble);
		}
	}
}