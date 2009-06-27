using System.IO;
using Xunit;

namespace ifpfc.VM
{
	public class Compiler_Test
	{
		[Fact]
		public void Disassemble()
		{
			string code = new Compiler(TestData.Hohmann).Compile("HohmannsEngine");
			File.WriteAllText(@"..\..\HohmannEngine.cs", code);
		}
	}
}