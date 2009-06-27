using System;
using System.Text;
using ifpfc.VM;

namespace ifpfc
{
	public class Compiler
	{
		private readonly string code =
			@"using System;
using System.Collections.Generic;
using System.Linq;
using ifpfc.Logic;
using ifpfc.VM;

namespace ifpfc
{
	public class {CLASSNAME} : IVirtualMachine
	{
		private readonly int teamId;
		private readonly int scenarioId;
		private const int addressSize = 14;
		private const int addressSpaceSize = 1 << addressSize;
		private bool status;
		private readonly List<double> dxs = new List<double>();
		private readonly List<double> dys = new List<double>();
		private int tickCount;

		public double[] Outport { get; private set; }
		public double[] Inport { get; private set; }
		public double[] Mem { get; private set; }
		

		public {CLASSNAME}(int teamId, int scenarioId, double configurationNumber)
		{
			this.teamId = teamId;
			this.scenarioId = scenarioId;
			Mem = new double[addressSpaceSize];
			Inport = new double[addressSpaceSize];
			Outport = new double[addressSpaceSize];
			Inport[16000] = configurationNumber;
			InitMem();
		}

		public double[] RunTimeStep(Vector dv)
		{
			dxs.Add(dv.x);
			dys.Add(dv.y);
			Inport[2] = dv.x;
			Inport[3] = dv.y;
			Execute();
			tickCount++;
			return Outport;
		}

		public byte[] CreateSubmission()
		{
			return
				BitConverter.GetBytes(0xCAFEBABE)
					.Concat(BitConverter.GetBytes(teamId))
					.Concat(BitConverter.GetBytes(scenarioId))
					.Concat(CreateFrames())
					.Concat(BitConverter.GetBytes(tickCount))
					.Concat(BitConverter.GetBytes(0))
					.ToArray();
		}

		private IEnumerable<byte> CreateFrames()
		{
			return
				from tick in Enumerable.Range(0, tickCount)
				let portList = CreatePortList(tick)
				where portList.Count > 0
				from b in CreateFrame(tick, portList)
				select b;
		}

		private IEnumerable<byte> CreateFrame(int tick, IList<int> portList)
		{
			var portBytes =
				from portNum in portList
				from b in
					BitConverter.GetBytes(portNum)
					.Concat(BitConverter.GetBytes(Inport[portNum]))
				select b;
			return
				BitConverter.GetBytes(tick)
					.Concat(BitConverter.GetBytes(portList.Count))
					.Concat(portBytes);
		}

		private IList<int> CreatePortList(int tick)
		{
			var result = new List<int>();
			if (tick == 0)
				result.Add(0x3E80);
			if (ShouldIncludePort(tick, dxs))
				result.Add(2);
			if (ShouldIncludePort(tick, dys))
				result.Add(3);
			return result;
		}

		private static bool ShouldIncludePort(int tick, IList<double> portValues)
		{
			return (tick == 0) || (portValues[tick - 1] != portValues[tick]);
		}

		private void InitMem()
		{
			//Generated code
			{INITMEM}
		}

		private void Execute()
		{
			//Generated code
			{EXECUTE}
		}
	}
}";
		public Compiler(byte[] initialImage)
		{
			imageSize = initialImage.Length;
			ImportImage(initialImage);
		}

		public string Compile(string className)
		{
			return code.Replace("{INITMEM}", GetInitMem()).Replace("{EXECUTE}", GetExecute()).Replace("{CLASSNAME}", className);
		}

		public string GetInitMem()
		{
			var builder = new StringBuilder();
			for (int i = 0; i < imageSize; i++)
				if (mem[i] != 0.0)
					builder.AppendFormat("Mem[" + i + "] = BitConverter.Int64BitsToDouble(" + mem[i] + ");\r\n");
			return builder.ToString();
		}

		public string GetExecute()
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
				bool even = offset % 2 == 0;
				int dataOffset = even ? 0 : 4;
				int instrOffset = even ? 8 : 0;
				
				mem[offset] = BitConverter.ToInt64(initialImage, start + dataOffset);
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
				builder.Append(ReadSType(address, op, imm, r1));
			}
			else
			{
				uint r1 = instr.ExtractBits(14, addressSize);
				uint r2 = instr.ExtractBits(0, addressSize);
				builder.Append(ReadDType(address, descriptor, r1, r2));
			}
		}

		private static string ReadSType(int rd, uint op, uint imm, uint r1)
		{
			switch (op)
			{
				case 0:
					return "";
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
					return string.Format("status = Mem[{0}] {1} 0.0;\r\n", r1, cmpOp);
				case 2:
					return string.Format("Mem[{0}] = Math.Sqrt(Mem[{1}]);\r\n", rd, r1);
				case 3:
					return string.Format("Mem[{0}] = Mem[{1}];\r\n", rd, r1);
				case 4:
					return string.Format("Mem[{0}] = Inport[{1}];\r\n", rd, r1);
				default:
					throw new Exception(string.Format("Неизвестный опкод одноаргументной операции {0}", op));
			}
		}

		private static string ReadDType(int rd, uint op, uint r1, uint r2)
		{
			Func<char, string> FormatBinOp = o => string.Format("Mem[{0}] = Mem[{1}] {2} Mem[{3}];\r\n", rd, r1, o, r2);

			switch (op)
			{
				case 1:
					return FormatBinOp('+');
				case 2:
					return FormatBinOp('-');
				case 3:
					return FormatBinOp('*');
				case 4:
					return FormatBinOp('/');
				case 5:
					return string.Format("Outport[{0}] = Mem[{1}];\r\n", r1, r2);
				case 6:
					return string.Format("Mem[{0}] = status ? Mem[{1}] : Mem[{2}];\r\n", rd, r1, r2);
				default:
					throw new Exception(string.Format("Неизвестный опкод двухаргументной операции {0}", op));
			}
		}

		private const int addressSize = 14;
		private const int addressSpaceSize = 1 << addressSize;
		private readonly uint[] instructions = new uint[addressSpaceSize];
		private readonly Int64[] mem = new Int64[addressSpaceSize];
		private readonly int imageSize;
	}
}