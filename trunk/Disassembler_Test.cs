﻿using System;
using Xunit;

namespace ifpfc
{
	public class Disassembler_Test
	{
		[Fact]
		public void Disassemble()
		{
			var disassemble = new Disasembler(TestData.Hohmann).Disassemble();
			Console.WriteLine(disassemble);
		}
	}
}