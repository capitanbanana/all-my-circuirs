using System;
using System.Collections.Generic;
using log4net;
using System.Linq;

namespace ifpfc
{
	public class VirtualMachine : IVirtualMachine
	{
		public VirtualMachine(int teamId, int scenarioId, double configurationNumber, byte[] initialImage)
		{
			this.teamId = teamId;
			this.scenarioId = scenarioId;
			inport[configurationPort] = configurationNumber;
			imageSize = initialImage.Length;
			ImportImage(initialImage);
		}

		public int TickCount
		{
			get { return tickCount; }
		}

		public double[] RunTimeStep(double dx, double dy)
		{
			dxs.Add(dx);
			dys.Add(dy);
			inport[dxPort] = dx;
			inport[dyPort] = dy;
			for (int i = 0; i < imageSize; i++)
				RunInstruction(i);
			tickCount++;
			return outport;
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
					.Concat(BitConverter.GetBytes(inport[portNum]))
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
				result.Add(configurationPort);
			if (ShouldIncludePort(tick, dxs))
				result.Add(dxPort);
			if (ShouldIncludePort(tick, dys))
				result.Add(dyPort);
			return result;
		}

		private static bool ShouldIncludePort(int tick, IList<double> portValues)
		{
			return (tick == 0) || (portValues[tick - 1] != portValues[tick]);
		}

		private void ImportImage(byte[] initialImage)
		{
			for(int start = 0, offset = 0; start < initialImage.Length; start += 12, offset++)
			{
				bool even = offset % 2 == 0;
				int dataOffset = even ? 0 : 4;
				int instrOffset = even ? 8 : 0;
				mem[offset] = BitConverter.ToDouble(initialImage, start + dataOffset);
				instructions[offset] = BitConverter.ToUInt32(initialImage, start + instrOffset);
			}
		}

		private void RunInstruction(int address)
		{
			uint instr = instructions[address];
			uint descriptor = instr.ExtractBits(28, 4);
			if (descriptor == 0)
			{
				uint op = instr.ExtractBits(24, 4);
				uint imm = instr.ExtractBits(21, 3);
				uint r1 = instr.ExtractBits(0, addressSize);
				RunSType(address, op, imm, r1);
			}
			else
			{
				uint r1 = instr.ExtractBits(14, addressSize);
				uint r2 = instr.ExtractBits(0, addressSize);
				RunDType(address, descriptor, r1, r2);
			}
		}

		private void RunSType(int rd, uint op, uint imm, uint r1)
		{
			string readableInstr;
			switch(op)
			{
				case 0:
					readableInstr = "nop";
					break;
				case 1:
					string cmpOp;
					double toCompare = mem[r1];
					switch(imm)
					{
						case 0:
							status = toCompare < 0.0;
							cmpOp = "<";
							break;
						case 1:
							status = toCompare <= 0.0;
							cmpOp = "<=";
							break;
						case 2:
							status = toCompare == 0.0;
							cmpOp = "==";
							break;
						case 3:
							status = toCompare >= 0.0;
							cmpOp = ">=";
							break;
						case 4:
							status = toCompare > 0.0;
							cmpOp = ">";
							break;
						default:
							throw new Exception(string.Format("Неизвестная операция сравнения {0}", imm));
					}
					readableInstr = string.Format("status <- {0} {1} 0.0", memAtR1, cmpOp);
					break;
				case 2:
					if (mem[r1] < 0)
						throw new Exception(string.Format("Вычисление квадратного корня от отрицательного числа {0}", mem[r1]));
					mem[rd] = Math.Sqrt(mem[r1]);
					readableInstr = string.Format("{0} <- sqrt({1})", memAtRd, memAtR1);
					break;
				case 3:
					mem[rd] = mem[r1];
					readableInstr = string.Format("{0} <- {1}", memAtRd, memAtR1);
					break;
				case 4:
					mem[rd] = inport[r1];
					readableInstr = string.Format("{0} <- {1}", memAtRd, inportAtR1);
					break;
				default:
					throw new Exception(string.Format("Неизвестный опкод одноаргументной операции {0}", op));
			}
			LogInstruction("S", readableInstr, rd, r1, null, op == 4, false);
		}

		private void RunDType(int rd, uint op, uint r1, uint r2)
		{
			string readableInstr;
			switch(op)
			{
				case 1:
					mem[rd] = mem[r1] + mem[r2];
					readableInstr = FormatBinOp("+");
					break;
				case 2:
					mem[rd] = mem[r1] - mem[r2];
					readableInstr = FormatBinOp("-");
					break;
				case 3:
					mem[rd] = mem[r1] * mem[r2];
					readableInstr = FormatBinOp("*");
					break;
				case 4:
					mem[rd] = (mem[r2] == 0.0) ? 0.0 : (mem[r1] / mem[r2]);
					readableInstr = FormatBinOp("/");
					break;
				case 5:
					outport[r1] = mem[r2];
					readableInstr = string.Format("{0} <- {1}", outportAtR1, memAtR2);
					break;
				case 6:
					mem[rd] = status ? mem[r1] : mem[r2];
					readableInstr = string.Format("{0} <- {1} ? {2} : {3}", memAtRd, status, memAtR1, memAtR2);
					break;
				default:
					throw new Exception(string.Format("Неизвестный опкод двухаргументной операции {0}", op));
			}
			LogInstruction("D", readableInstr, rd, r1, r2, false, op == 5);
		}

		private static string FormatBinOp(string binOp)
		{
			return string.Format("{0} <- {1} {2} {3}", memAtRd, memAtR1, binOp, memAtR2);
		}

		private static string FormatAddress(int address)
		{
			return string.Format("{0:D5}", address);
		}

		private static string FormatAddress(uint address)
		{
			return FormatAddress((int)address);
		}

		private void LogInstruction(string type, string readableInstr, int rd, uint r1, uint? r2, bool includeInport, bool includeOutport)
		{
			string r1DebugInfo =
				includeInport ? (inportAtR1 + " = " + inport[r1]) :
				includeOutport ? (outportAtR1 + " = " + outport[r1]) :
				memAtR1 + " = " + mem[r1];
			string debugInfo = string.Format("r1 = {0}, {1}", FormatAddress(r1), r1DebugInfo);
			if (r2 != null)
				debugInfo += string.Format(", r2 = {0}, {1} = {2}", FormatAddress(r2.Value), memAtR2, mem[r2.Value]);
			debugInfo += string.Format(", rd = {0}, {1} = {2}", FormatAddress(rd), memAtRd, mem[rd]);
			log.InfoFormat("{0}{1} {2,-40} ({3})", type, FormatAddress(rd), readableInstr, debugInfo);
		}

		private const int addressSize = 14;
		private const int addressSpaceSize = 1 << addressSize;
		private const string memAtRd = "mem[rd]";
		private const string memAtR1 = "mem[r1]";
		private const string memAtR2 = "mem[r2]";
		private const string inportAtR1 = "inport[r1]";
		private const string outportAtR1 = "outport[r1]";
		private const int configurationPort = 0x3E80;
		private const int dxPort = 0x2;
		private const int dyPort = 0x3;

		private readonly uint[] instructions = new uint[addressSpaceSize];
		private readonly double[] mem = new double[addressSpaceSize];
		private readonly double[] inport = new double[addressSpaceSize];
		private readonly double[] outport = new double[addressSpaceSize];

		private int tickCount;
		private bool status;

		private readonly int teamId;
		private readonly int scenarioId;

		private readonly List<double> dxs = new List<double>();
		private readonly List<double> dys = new List<double>();

		private static readonly ILog log = LogManager.GetLogger(typeof(VirtualMachine));
		private readonly int imageSize;
	}
}