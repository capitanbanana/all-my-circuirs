using System;
using System.Collections.Generic;
using System.Linq;
using ifpfc.Logic;
using ifpfc.VM;

namespace ifpfc.VM
{
	public class HohmannsEngine : IVirtualMachine
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
		

		public HohmannsEngine(int teamId, int scenarioId)
		{
			this.teamId = teamId;
			this.scenarioId = scenarioId;
			Mem = new double[addressSpaceSize];
			Inport = new double[addressSpaceSize];
			Outport = new double[addressSpaceSize];
			Inport[16000] = scenarioId;
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
			Func<int, double> getPortValue =
				portNum =>
				{
					if(portNum == 0x3E80) return Inport[0x3E80];
					if(portNum == 2) return dxs[tick];
					if(portNum == 3) return dys[tick];
					throw new Exception(portNum.ToString());
				};

			var portBytes =
				from portNum in portList
				from b in
					BitConverter.GetBytes(portNum)
					.Concat(BitConverter.GetBytes(getPortValue(portNum)))
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
			Mem[0] = BitConverter.Int64BitsToDouble(4607182418800017408);
Mem[2] = BitConverter.Int64BitsToDouble(4629137466983448576);
Mem[16] = BitConverter.Int64BitsToDouble(4652007308841189376);
Mem[19] = BitConverter.Int64BitsToDouble(4611686018427387904);
Mem[27] = BitConverter.Int64BitsToDouble(4607632778762754458);
Mem[28] = BitConverter.Int64BitsToDouble(4676027789617397760);
Mem[31] = BitConverter.Int64BitsToDouble(4652042493213278208);
Mem[36] = BitConverter.Int64BitsToDouble(4609434218613702656);
Mem[38] = BitConverter.Int64BitsToDouble(4652033697120256000);
Mem[43] = BitConverter.Int64BitsToDouble(4652024901027233792);
Mem[47] = BitConverter.Int64BitsToDouble(4652016104934211584);
Mem[55] = BitConverter.Int64BitsToDouble(4708691161932365824);
Mem[60] = BitConverter.Int64BitsToDouble(-4514788249104809984);
Mem[78] = BitConverter.Int64BitsToDouble(4710731271397965824);
Mem[81] = BitConverter.Int64BitsToDouble(4708583787749965824);
Mem[84] = BitConverter.Int64BitsToDouble(4708798536114765824);
Mem[96] = BitConverter.Int64BitsToDouble(4977561924064720455);
Mem[99] = BitConverter.Int64BitsToDouble(4454720405870426065);
Mem[111] = BitConverter.Int64BitsToDouble(-4559038833935623131);
Mem[114] = BitConverter.Int64BitsToDouble(-4561461077237492124);
Mem[117] = BitConverter.Int64BitsToDouble(-4558057412737400149);
Mem[133] = BitConverter.Int64BitsToDouble(-4557991131213599552);
Mem[189] = BitConverter.Int64BitsToDouble(4607182418800017408);
Mem[214] = BitConverter.Int64BitsToDouble(4666723172467343360);
Mem[219] = BitConverter.Int64BitsToDouble(4627730092099895296);
Mem[220] = BitConverter.Int64BitsToDouble(4631530004285489152);
Mem[226] = BitConverter.Int64BitsToDouble(4651127699538968576);
Mem[238] = BitConverter.Int64BitsToDouble(4708583787749965824);

		}

		private void Execute()
		{
			//Generated code
			Mem[1] = Mem[265];
Mem[4] = Mem[248];
Mem[5] = Mem[4] - Mem[3];
status = Mem[5] == 0.0;
Mem[7] = status ? Mem[2] : Mem[1];
Mem[8] = Mem[7] - Mem[0];
Mem[9] = Mem[263];
status = Mem[5] == 0.0;
Mem[11] = status ? Mem[0] : Mem[9];
Mem[12] = Mem[11] - Mem[0];
status = Mem[12] == 0.0;
Mem[14] = status ? Mem[8] : Mem[7];
Mem[15] = Mem[264];
status = Mem[5] == 0.0;
Mem[18] = status ? Mem[16] : Mem[15];
Mem[20] = Mem[18] * Mem[19];
status = Mem[12] == 0.0;
Mem[22] = status ? Mem[20] : Mem[18];
status = Mem[12] == 0.0;
Mem[24] = status ? Mem[18] : Mem[12];
Mem[25] = Mem[260];
Mem[26] = Mem[262];
Mem[29] = Mem[28] * Mem[16];
Mem[30] = Mem[29] / Mem[27];
Mem[32] = Inport[16000];
Mem[33] = Mem[32] - Mem[31];
status = Mem[33] == 0.0;
Mem[35] = status ? Mem[30] : Mem[3];
Mem[37] = Mem[29] / Mem[36];
Mem[39] = Mem[32] - Mem[38];
status = Mem[39] == 0.0;
Mem[41] = status ? Mem[37] : Mem[35];
Mem[42] = Mem[29] / Mem[19];
Mem[44] = Mem[32] - Mem[43];
status = Mem[44] == 0.0;
Mem[46] = status ? Mem[42] : Mem[41];
Mem[48] = Mem[32] - Mem[47];
status = Mem[48] == 0.0;
Mem[50] = status ? Mem[29] : Mem[46];
status = Mem[5] == 0.0;
Mem[52] = status ? Mem[50] : Mem[26];
Mem[53] = Mem[255];
status = Mem[33] == 0.0;
Mem[57] = status ? Mem[55] : Mem[3];
status = Mem[39] == 0.0;
Mem[59] = status ? Mem[54] : Mem[57];
status = Mem[44] == 0.0;
Mem[62] = status ? Mem[60] : Mem[59];
status = Mem[48] == 0.0;
Mem[64] = status ? Mem[54] : Mem[62];
status = Mem[5] == 0.0;
Mem[66] = status ? Mem[64] : Mem[53];
Mem[67] = Mem[250];
status = Mem[5] == 0.0;
Mem[69] = status ? Mem[3] : Mem[67];
Mem[70] = Mem[69] - Mem[66];
Mem[71] = Mem[70] * Mem[70];
Mem[72] = Mem[249];
status = Mem[5] == 0.0;
Mem[74] = status ? Mem[3] : Mem[72];
Mem[75] = Mem[254];
status = Mem[33] == 0.0;
Mem[77] = status ? Mem[54] : Mem[3];
status = Mem[39] == 0.0;
Mem[80] = status ? Mem[78] : Mem[77];
status = Mem[44] == 0.0;
Mem[83] = status ? Mem[81] : Mem[80];
status = Mem[48] == 0.0;
Mem[86] = status ? Mem[84] : Mem[83];
status = Mem[5] == 0.0;
Mem[88] = status ? Mem[86] : Mem[75];
Mem[89] = Mem[74] - Mem[88];
Mem[90] = Mem[89] * Mem[89];
Mem[91] = Mem[90] + Mem[71];
Mem[92] = Math.Sqrt(Mem[91]);
Mem[93] = Mem[92] * Mem[92];
Mem[94] = Mem[93] * Mem[92];
Mem[95] = Mem[251];
status = Mem[5] == 0.0;
Mem[98] = status ? Mem[96] : Mem[95];
Mem[100] = Mem[99] * Mem[98];
Mem[101] = Mem[100] / Mem[94];
Mem[102] = Mem[70] * Mem[101];
Mem[103] = Inport[3];
Mem[104] = Mem[0] / Mem[0];
Mem[105] = Mem[103] / Mem[104];
Mem[106] = Mem[105] + Mem[102];
Mem[107] = Mem[104] * Mem[104];
Mem[108] = Mem[107] / Mem[19];
Mem[109] = Mem[106] * Mem[108];
Mem[110] = Mem[258];
status = Mem[39] == 0.0;
Mem[113] = status ? Mem[111] : Mem[77];
status = Mem[44] == 0.0;
Mem[116] = status ? Mem[114] : Mem[113];
status = Mem[48] == 0.0;
Mem[119] = status ? Mem[117] : Mem[116];
status = Mem[5] == 0.0;
Mem[121] = status ? Mem[119] : Mem[110];
Mem[122] = Mem[121] * Mem[104];
Mem[123] = Mem[66] + Mem[122];
Mem[124] = Mem[123] + Mem[109];
Mem[125] = Mem[124] - Mem[69];
Mem[126] = Mem[125] * Mem[125];
Mem[127] = Mem[89] * Mem[101];
Mem[128] = Inport[2];
Mem[129] = Mem[128] / Mem[104];
Mem[130] = Mem[129] + Mem[127];
Mem[131] = Mem[130] * Mem[108];
Mem[132] = Mem[257];
status = Mem[33] == 0.0;
Mem[135] = status ? Mem[133] : Mem[3];
status = Mem[39] == 0.0;
Mem[137] = status ? Mem[54] : Mem[135];
status = Mem[44] == 0.0;
Mem[139] = status ? Mem[114] : Mem[137];
status = Mem[48] == 0.0;
Mem[141] = status ? Mem[54] : Mem[139];
status = Mem[5] == 0.0;
Mem[143] = status ? Mem[141] : Mem[132];
Mem[144] = Mem[143] * Mem[104];
Mem[145] = Mem[88] + Mem[144];
Mem[146] = Mem[145] + Mem[131];
Mem[147] = Mem[146] - Mem[74];
Mem[148] = Mem[147] * Mem[147];
Mem[149] = Mem[148] + Mem[126];
Mem[150] = Math.Sqrt(Mem[149]);
Mem[151] = Mem[150] - Mem[52];
Mem[152] = Mem[3] - Mem[151];
Mem[153] = Mem[151] - Mem[3];
status = Mem[153] < 0.0;
Mem[155] = status ? Mem[152] : Mem[151];
Mem[156] = Mem[25] + Mem[155];
Mem[157] = Mem[105] * Mem[105];
Mem[158] = Mem[129] * Mem[129];
Mem[159] = Mem[158] + Mem[157];
Mem[160] = Math.Sqrt(Mem[159]);
Mem[161] = Mem[160] - Mem[3];
status = Mem[161] == 0.0;
Mem[163] = status ? Mem[156] : Mem[3];
Mem[164] = Mem[155] - Mem[16];
status = Mem[164] < 0.0;
Mem[166] = status ? Mem[163] : Mem[3];
Mem[167] = Mem[69] - Mem[124];
Mem[168] = Mem[167] * Mem[167];
Mem[169] = Mem[74] - Mem[146];
Mem[170] = Mem[169] * Mem[169];
Mem[171] = Mem[170] + Mem[168];
Mem[172] = Math.Sqrt(Mem[171]);
Mem[173] = Mem[172] * Mem[172];
Mem[174] = Mem[173] * Mem[172];
Mem[175] = Mem[100] / Mem[174];
Mem[176] = Mem[167] * Mem[175];
Mem[177] = Mem[176] + Mem[102];
Mem[178] = Mem[177] / Mem[19];
Mem[179] = Mem[105] + Mem[178];
Mem[180] = Mem[179] * Mem[104];
Mem[181] = Mem[121] + Mem[180];
Mem[182] = Mem[169] * Mem[175];
Mem[183] = Mem[182] + Mem[127];
Mem[184] = Mem[183] / Mem[19];
Mem[185] = Mem[129] + Mem[184];
Mem[186] = Mem[185] * Mem[104];
Mem[187] = Mem[143] + Mem[186];
Mem[188] = Mem[256];
status = Mem[33] == 0.0;
Mem[191] = status ? Mem[189] : Mem[3];
status = Mem[39] == 0.0;
Mem[193] = status ? Mem[189] : Mem[191];
status = Mem[44] == 0.0;
Mem[195] = status ? Mem[189] : Mem[193];
status = Mem[48] == 0.0;
Mem[197] = status ? Mem[189] : Mem[195];
status = Mem[5] == 0.0;
Mem[199] = status ? Mem[197] : Mem[188];
Mem[200] = Mem[253];
status = Mem[5] == 0.0;
Mem[202] = status ? Mem[3] : Mem[200];
Mem[203] = Mem[252];
status = Mem[5] == 0.0;
Mem[205] = status ? Mem[3] : Mem[203];
Mem[206] = Mem[4] + Mem[0];
Mem[207] = Mem[259];
Mem[208] = Mem[207] + Mem[0];
status = Mem[161] == 0.0;
Mem[210] = status ? Mem[208] : Mem[3];
status = Mem[164] < 0.0;
Mem[212] = status ? Mem[210] : Mem[3];
Mem[213] = Mem[261];
status = Mem[5] == 0.0;
Mem[216] = status ? Mem[214] : Mem[213];
Mem[217] = Mem[160] * Mem[104];
Mem[218] = Mem[216] - Mem[217];
Mem[221] = Mem[214] - Mem[218];
Mem[222] = Mem[221] / Mem[214];
Mem[223] = Mem[222] * Mem[220];
Mem[224] = Mem[7] + Mem[223];
Mem[225] = Mem[224] + Mem[219];
Mem[227] = Mem[226] / Mem[104];
Mem[228] = Mem[227] - Mem[212];
status = Mem[228] < 0.0;
Mem[230] = status ? Mem[225] : Mem[3];
Mem[231] = Mem[218] - Mem[3];
Mem[232] = Mem[242] - Mem[0];
status = Mem[231] < 0.0;
Mem[234] = status ? Mem[232] : Mem[230];
Mem[235] = Mem[214] - Mem[217];
status = Mem[235] < 0.0;
Mem[237] = status ? Mem[232] : Mem[234];
Mem[239] = Mem[150] - Mem[238];
status = Mem[239] < 0.0;
Mem[241] = status ? Mem[232] : Mem[237];
Outport[0] = Mem[241];
Outport[1] = Mem[218];
Outport[2] = Mem[169];
Outport[3] = Mem[167];
Outport[4] = Mem[52];
Mem[248] = Mem[206];
Mem[249] = Mem[74];
Mem[250] = Mem[69];
Mem[251] = Mem[98];
Mem[252] = Mem[205];
Mem[253] = Mem[202];
Mem[254] = Mem[146];
Mem[255] = Mem[124];
Mem[256] = Mem[199];
Mem[257] = Mem[187];
Mem[258] = Mem[181];
Mem[259] = Mem[212];
Mem[260] = Mem[166];
Mem[261] = Mem[218];
Mem[262] = Mem[52];
Mem[263] = Mem[24];
Mem[264] = Mem[22];
Mem[265] = Mem[14];

		}
	}
}