using System;

namespace ifpfc
{
	public class VirtualMachine : IVirtualMachine
	{
		public VirtualMachine(int teamId, int scenarioId, double configurationNumber, byte[] initialImage)
		{
			this.teamId = teamId;
			this.scenarioId = scenarioId;
			inport[0x3E80] = configurationNumber;
			ImportImage(initialImage);
		}

		public double[] RunTimeStep(double dx, double dy)
		{
			inport[0x2] = dx;
			inport[0x3] = dy;
			for (int i = 0; i < addressSpaceSize; i++)
				RunInstruction(i);
			timeStep++;
			return outport;
		}

		public byte[] FormSubmission()
		{
			throw new NotImplementedException();
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
				uint imm = instr.ExtractBits(14, 10);
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
			switch(op)
			{
				case 0:
					break;
				case 1:
					double toCompare = mem[r1];
					switch(imm)
					{
						case 0:
							status = toCompare < 0.0;
							break;
						case 1:
							status = toCompare <= 0.0;
							break;
						case 2:
							status = toCompare == 0.0;
							break;
						case 3:
							status = toCompare >= 0.0;
							break;
						case 4:
							status = toCompare > 0.0;
							break;
					}
					break;
				case 2:
					if (mem[r1] < 0)
						throw new Exception(string.Format("Вычисление квадратного корня от отрицательного числа {0}", mem[r1]));
					mem[rd] = Math.Sqrt(mem[r1]);
					break;
				case 3:
					mem[rd] = mem[r1];
					break;
				case 4:
					mem[rd] = inport[r1];
					break;
				default:
					throw new Exception(string.Format("Неизвестный опкод одноаргументной операции {0}", op));
			}
		}

		private void RunDType(int rd, uint op, uint r1, uint r2)
		{
			switch(op)
			{
				case 1:
					mem[rd] = mem[r1] + mem[r2];
					break;
				case 2:
					mem[rd] = mem[r1] - mem[r2];
					break;
				case 3:
					mem[rd] = mem[r1] * mem[r2];
					break;
				case 4:
					mem[rd] = (mem[r2] == 0.0) ? 0.0 : mem[r1] / mem[r2];
					break;
				case 5:
					outport[r1] = mem[r2];
					break;
				case 6:
					mem[rd] = status ? mem[r1] : mem[r2];
					break;
				default:
					throw new Exception(string.Format("Неизвестный опкод двухаргументной операции {0}", op));
			}
		}

		const int addressSize = 14;
		const int addressSpaceSize = 1 << addressSize;

		private readonly uint[] instructions = new uint[addressSpaceSize];
		private readonly double[] mem = new double[addressSpaceSize];
		private readonly double[] inport = new double[addressSpaceSize];
		private readonly double[] outport = new double[addressSpaceSize];

		private int timeStep;
		private bool status;

		private readonly int teamId;
		private readonly int scenarioId;
	}
}