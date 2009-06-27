using System;
using Xunit;
using ifpfc.VM;

namespace ifpfc
{
	public class Compiler_Test
	{
		[Fact]
		public void Disassemble()
		{
			var code = new Compiler(TestData.Hohmann).Compile("HohmannsEngine");
			System.IO.File.WriteAllText(@"..\..\HohmannEngine.cs", code);
		}
	}

}


