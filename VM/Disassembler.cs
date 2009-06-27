using System;
using System.Text;

namespace ifpfc.VM
{
	public class Disasembler
	{
		private const int addressSize = 14;
		private const int addressSpaceSize = 1 << addressSize;
		private readonly int imageSize;
		private readonly uint[] instructions = new uint[addressSpaceSize];
		private readonly double[] mem = new double[addressSpaceSize];

		public Disasembler(byte[] initialImage)
		{
			imageSize = initialImage.Length;
			ImportImage(initialImage);
		}

		public string Disassemble()
		{
			var builder = new StringBuilder();
			for (int i = 0; i < imageSize; i++)
				ReadInstruction(i, builder);
			return builder.ToString();
		}

		private void ImportImage(byte[] initialImage)
		{
			for (int start = 0, offset = 0; start < initialImage.Length; start += 12, offset++)
			{
				bool even = offset%2 == 0;
				int dataOffset = even ? 0 : 4;
				int instrOffset = even ? 8 : 0;
				mem[offset] = BitConverter.ToDouble(initialImage, start + dataOffset);
				instructions[offset] = BitConverter.ToUInt32(initialImage, start + instrOffset);
			}
		}

		private void ReadInstruction(int address, StringBuilder builder)
		{
			uint instr = instructions[address];
			uint descriptor = instr.ExtractBits(28, 4);
			if (descriptor == 0)
			{
				uint op = instr.ExtractBits(24, 4);
				uint imm = instr.ExtractBits(21, 3);
				uint r1 = instr.ExtractBits(0, addressSize);
				builder.AppendLine(ReadSType(address, op, imm, r1));
			}
			else
			{
				uint r1 = instr.ExtractBits(14, addressSize);
				uint r2 = instr.ExtractBits(0, addressSize);
				builder.AppendLine(ReadDType(address, descriptor, r1, r2));
			}
		}

		private string ReadSType(int rd, uint op, uint imm, uint r1)
		{
			string readableInstr;
			switch (op)
			{
				case 0:
					readableInstr = "nop";
					break;
				case 1:
					string cmpOp;
					switch (imm)
					{
						case 0:
							cmpOp = "<";
							break;
						case 1:
							cmpOp = "<=";
							break;
						case 2:
							cmpOp = "==";
							break;
						case 3:
							cmpOp = ">=";
							break;
						case 4:
							cmpOp = ">";
							break;
						default:
							throw new Exception(string.Format("Неизвестная операция сравнения {0}", imm));
					}
					readableInstr = string.Format("status <- mem[{0}] {1} 0.0", r1, cmpOp);
					break;
				case 2:
					readableInstr = string.Format("mem[{0}] <- sqrt(mem[{1}])", rd, r1);
					break;
				case 3:
					readableInstr = string.Format("mem[{0}] <- mem[{1}]", rd, r1);
					break;
				case 4:
					readableInstr = string.Format("mem[{0}] <- inport[{1}]", rd, r1);
					break;
				default:
					throw new Exception(string.Format("Неизвестный опкод одноаргументной операции {0}", op));
			}
			return string.Format("{0}\t{1}\t\tmem: {2}", FormatAddress(rd), readableInstr.PadRight(40, ' '), mem[rd]);
		}

		private string ReadDType(int rd, uint op, uint r1, uint r2)
		{
			string readableInstr;
			Func<char, string> FormatBinOp = o => string.Format("mem[{0}] <- mem[{1}] {2} mem[{3}]", rd, r1, o, r2);

			switch (op)
			{
				case 1:
					readableInstr = FormatBinOp('+');
					break;
				case 2:
					readableInstr = FormatBinOp('-');
					break;
				case 3:
					readableInstr = FormatBinOp('*');
					break;
				case 4:
					readableInstr = FormatBinOp('/');
					break;
				case 5:
					readableInstr = string.Format("outport[{0}] <- mem[{1}]", r1, r2);
					break;
				case 6:
					readableInstr = string.Format("mem[{0}] <- status ? mem[{1}] : mem[{2}]", rd, r1, r2);
					break;
				default:
					throw new Exception(string.Format("Неизвестный опкод двухаргументной операции {0}", op));
			}
			return string.Format("{0}\t{1}\t\tmem: {2}", FormatAddress(rd), readableInstr.PadRight(40, ' '), mem[rd]);
		}

		private static string FormatAddress(int address)
		{
			return string.Format("{0:D5}", address);
		}
	}
}