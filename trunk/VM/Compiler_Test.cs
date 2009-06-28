using System.IO;
using Xunit;

namespace ifpfc.VM
{
	public class Compiler_Test
	{
		[Fact]
		public void DisassembleHohmann()
		{
			string code = new Compiler(TestData.Hohmann).Compile("HohmannsEngine");
			File.WriteAllText(@"..\..\VM\HohmannEngine.cs", code);
		}

		[Fact]
		public void DisassembleMeetAndGreet()
		{
			string code = new Compiler(TestData.MeetAndGreet).Compile("MeetAndGreetEngine");
			File.WriteAllText(@"..\..\VM\MeetAndGreetEngine.cs", code);
		}
		[Fact]
		public void DisassembleEccentricMeetAndGreet()
		{
			string code = new Compiler(TestData.MeetAndGreet).Compile("EccentricMeetAndGreetEngine");
			File.WriteAllText(@"..\..\VM\EccentricMeetAndGreetEngine.cs", code);
		}
	}
}