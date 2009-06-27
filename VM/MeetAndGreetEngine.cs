using System;
using System.Collections.Generic;
using System.Linq;
using ifpfc.Logic;
using ifpfc.VM;

namespace ifpfc.VM
{
	public class MeetAndGreetEngine : IVirtualMachine
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
		

		public MeetAndGreetEngine(int teamId, int scenarioId, double configurationNumber)
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
			Mem[0] = BitConverter.Int64BitsToDouble(4607182418800017408);
Mem[2] = BitConverter.Int64BitsToDouble(4629137466983448576);
Mem[15] = BitConverter.Int64BitsToDouble(4611686018427387904);
Mem[17] = BitConverter.Int64BitsToDouble(4652007308841189376);
Mem[28] = BitConverter.Int64BitsToDouble(4710731271397965824);
Mem[29] = BitConverter.Int64BitsToDouble(4656528500654604288);
Mem[34] = BitConverter.Int64BitsToDouble(4656524102608093184);
Mem[38] = BitConverter.Int64BitsToDouble(-4514788249104809984);
Mem[39] = BitConverter.Int64BitsToDouble(4656519704561582080);
Mem[43] = BitConverter.Int64BitsToDouble(4656515306515070976);
Mem[57] = BitConverter.Int64BitsToDouble(4709657529573965824);
Mem[60] = BitConverter.Int64BitsToDouble(4723801030825869312);
Mem[76] = BitConverter.Int64BitsToDouble(4454720405870426065);
Mem[78] = BitConverter.Int64BitsToDouble(4977561924064720455);
Mem[89] = BitConverter.Int64BitsToDouble(-4558538034427889124);
Mem[92] = BitConverter.Int64BitsToDouble(-4565675388493619430);
Mem[95] = BitConverter.Int64BitsToDouble(-4559038833935623131);
Mem[104] = BitConverter.Int64BitsToDouble(4708691161932365824);
Mem[138] = BitConverter.Int64BitsToDouble(4708583787749965824);
Mem[141] = BitConverter.Int64BitsToDouble(4708798536114765824);
Mem[169] = BitConverter.Int64BitsToDouble(-4561461077237492124);
Mem[172] = BitConverter.Int64BitsToDouble(-4558057412737400149);
Mem[193] = BitConverter.Int64BitsToDouble(4664333202919152677);
Mem[198] = BitConverter.Int64BitsToDouble(-4580402847030641008);
Mem[214] = BitConverter.Int64BitsToDouble(4665380905641176256);
Mem[271] = BitConverter.Int64BitsToDouble(4607182418800017408);
Mem[338] = BitConverter.Int64BitsToDouble(4677104761256804352);
Mem[343] = BitConverter.Int64BitsToDouble(4627730092099895296);
Mem[344] = BitConverter.Int64BitsToDouble(4631530004285489152);
Mem[350] = BitConverter.Int64BitsToDouble(4651127699538968576);
Mem[362] = BitConverter.Int64BitsToDouble(4708583787749965824);

		}

		private void Execute()
		{
			//Generated code
			Mem[1] = Mem[400];
Mem[4] = Mem[379];
Mem[5] = Mem[4] - Mem[3];
status = Mem[5] == 0.0;
Mem[7] = status ? Mem[2] : Mem[1];
Mem[8] = Mem[7] - Mem[0];
Mem[9] = Mem[398];
status = Mem[5] == 0.0;
Mem[11] = status ? Mem[0] : Mem[9];
Mem[12] = Mem[11] - Mem[0];
status = Mem[12] == 0.0;
Mem[14] = status ? Mem[8] : Mem[7];
Mem[16] = Mem[399];
status = Mem[5] == 0.0;
Mem[19] = status ? Mem[17] : Mem[16];
Mem[20] = Mem[19] * Mem[15];
status = Mem[12] == 0.0;
Mem[22] = status ? Mem[20] : Mem[19];
status = Mem[12] == 0.0;
Mem[24] = status ? Mem[19] : Mem[12];
Mem[25] = Mem[396];
Mem[26] = Mem[391];
Mem[30] = Inport[16000];
Mem[31] = Mem[30] - Mem[29];
status = Mem[31] == 0.0;
Mem[33] = status ? Mem[28] : Mem[3];
Mem[35] = Mem[30] - Mem[34];
status = Mem[35] == 0.0;
Mem[37] = status ? Mem[27] : Mem[33];
Mem[40] = Mem[30] - Mem[39];
status = Mem[40] == 0.0;
Mem[42] = status ? Mem[38] : Mem[37];
Mem[44] = Mem[30] - Mem[43];
status = Mem[44] == 0.0;
Mem[46] = status ? Mem[27] : Mem[42];
status = Mem[5] == 0.0;
Mem[48] = status ? Mem[46] : Mem[26];
Mem[49] = Mem[381];
status = Mem[5] == 0.0;
Mem[51] = status ? Mem[3] : Mem[49];
Mem[52] = Mem[51] - Mem[48];
Mem[53] = Mem[52] * Mem[52];
Mem[54] = Mem[390];
status = Mem[31] == 0.0;
Mem[56] = status ? Mem[27] : Mem[3];
status = Mem[35] == 0.0;
Mem[59] = status ? Mem[57] : Mem[56];
status = Mem[40] == 0.0;
Mem[62] = status ? Mem[60] : Mem[59];
status = Mem[44] == 0.0;
Mem[64] = status ? Mem[28] : Mem[62];
status = Mem[5] == 0.0;
Mem[66] = status ? Mem[64] : Mem[54];
Mem[67] = Mem[380];
status = Mem[5] == 0.0;
Mem[69] = status ? Mem[3] : Mem[67];
Mem[70] = Mem[69] - Mem[66];
Mem[71] = Mem[70] * Mem[70];
Mem[72] = Mem[71] + Mem[53];
Mem[73] = Math.Sqrt(Mem[72]);
Mem[74] = Mem[73] * Mem[73];
Mem[75] = Mem[74] * Mem[73];
Mem[77] = Mem[382];
status = Mem[5] == 0.0;
Mem[80] = status ? Mem[78] : Mem[77];
Mem[81] = Mem[76] * Mem[80];
Mem[82] = Mem[81] / Mem[75];
Mem[83] = Mem[52] * Mem[82];
Mem[84] = Mem[0] / Mem[0];
Mem[85] = Mem[84] * Mem[84];
Mem[86] = Mem[85] / Mem[15];
Mem[87] = Mem[83] * Mem[86];
Mem[88] = Mem[394];
status = Mem[35] == 0.0;
Mem[91] = status ? Mem[89] : Mem[56];
status = Mem[40] == 0.0;
Mem[94] = status ? Mem[92] : Mem[91];
status = Mem[44] == 0.0;
Mem[97] = status ? Mem[95] : Mem[94];
status = Mem[5] == 0.0;
Mem[99] = status ? Mem[97] : Mem[88];
Mem[100] = Mem[99] * Mem[84];
Mem[101] = Mem[48] + Mem[100];
Mem[102] = Mem[101] + Mem[87];
Mem[103] = Mem[386];
Mem[105] = Mem[29] + Mem[17];
Mem[106] = Mem[30] - Mem[105];
status = Mem[106] == 0.0;
Mem[108] = status ? Mem[104] : Mem[3];
Mem[109] = Mem[34] + Mem[17];
Mem[110] = Mem[30] - Mem[109];
status = Mem[110] == 0.0;
Mem[112] = status ? Mem[27] : Mem[108];
Mem[113] = Mem[39] + Mem[17];
Mem[114] = Mem[30] - Mem[113];
status = Mem[114] == 0.0;
Mem[116] = status ? Mem[38] : Mem[112];
Mem[117] = Mem[43] + Mem[17];
Mem[118] = Mem[30] - Mem[117];
status = Mem[118] == 0.0;
Mem[120] = status ? Mem[27] : Mem[116];
status = Mem[31] == 0.0;
Mem[122] = status ? Mem[104] : Mem[120];
status = Mem[35] == 0.0;
Mem[124] = status ? Mem[27] : Mem[122];
status = Mem[40] == 0.0;
Mem[126] = status ? Mem[38] : Mem[124];
status = Mem[44] == 0.0;
Mem[128] = status ? Mem[27] : Mem[126];
status = Mem[5] == 0.0;
Mem[130] = status ? Mem[128] : Mem[103];
Mem[131] = Mem[51] - Mem[130];
Mem[132] = Mem[131] * Mem[131];
Mem[133] = Mem[385];
status = Mem[106] == 0.0;
Mem[135] = status ? Mem[27] : Mem[3];
status = Mem[110] == 0.0;
Mem[137] = status ? Mem[28] : Mem[135];
status = Mem[114] == 0.0;
Mem[140] = status ? Mem[138] : Mem[137];
status = Mem[118] == 0.0;
Mem[143] = status ? Mem[141] : Mem[140];
status = Mem[31] == 0.0;
Mem[145] = status ? Mem[27] : Mem[143];
status = Mem[35] == 0.0;
Mem[147] = status ? Mem[28] : Mem[145];
status = Mem[40] == 0.0;
Mem[149] = status ? Mem[138] : Mem[147];
status = Mem[44] == 0.0;
Mem[151] = status ? Mem[141] : Mem[149];
status = Mem[5] == 0.0;
Mem[153] = status ? Mem[151] : Mem[133];
Mem[154] = Mem[69] - Mem[153];
Mem[155] = Mem[154] * Mem[154];
Mem[156] = Mem[155] + Mem[132];
Mem[157] = Math.Sqrt(Mem[156]);
Mem[158] = Mem[157] * Mem[157];
Mem[159] = Mem[158] * Mem[157];
Mem[160] = Mem[81] / Mem[159];
Mem[161] = Mem[131] * Mem[160];
Mem[162] = Inport[3];
Mem[163] = Mem[162] / Mem[84];
Mem[164] = Mem[163] + Mem[161];
Mem[165] = Mem[164] * Mem[86];
Mem[166] = Mem[389];
status = Mem[110] == 0.0;
Mem[168] = status ? Mem[95] : Mem[135];
status = Mem[114] == 0.0;
Mem[171] = status ? Mem[169] : Mem[168];
status = Mem[118] == 0.0;
Mem[174] = status ? Mem[172] : Mem[171];
status = Mem[31] == 0.0;
Mem[176] = status ? Mem[27] : Mem[174];
status = Mem[35] == 0.0;
Mem[178] = status ? Mem[95] : Mem[176];
status = Mem[40] == 0.0;
Mem[180] = status ? Mem[169] : Mem[178];
status = Mem[44] == 0.0;
Mem[182] = status ? Mem[172] : Mem[180];
status = Mem[5] == 0.0;
Mem[184] = status ? Mem[182] : Mem[166];
Mem[185] = Mem[184] * Mem[84];
Mem[186] = Mem[130] + Mem[185];
Mem[187] = Mem[186] + Mem[165];
Mem[188] = Mem[187] - Mem[102];
Mem[189] = Mem[188] * Mem[188];
Mem[190] = Mem[70] * Mem[82];
Mem[191] = Mem[190] * Mem[86];
Mem[192] = Mem[393];
status = Mem[31] == 0.0;
Mem[195] = status ? Mem[193] : Mem[3];
status = Mem[35] == 0.0;
Mem[197] = status ? Mem[27] : Mem[195];
status = Mem[40] == 0.0;
Mem[200] = status ? Mem[198] : Mem[197];
status = Mem[44] == 0.0;
Mem[202] = status ? Mem[27] : Mem[200];
status = Mem[5] == 0.0;
Mem[204] = status ? Mem[202] : Mem[192];
Mem[205] = Mem[204] * Mem[84];
Mem[206] = Mem[66] + Mem[205];
Mem[207] = Mem[206] + Mem[191];
Mem[208] = Mem[154] * Mem[160];
Mem[209] = Inport[2];
Mem[210] = Mem[209] / Mem[84];
Mem[211] = Mem[210] + Mem[208];
Mem[212] = Mem[211] * Mem[86];
Mem[213] = Mem[388];
status = Mem[106] == 0.0;
Mem[216] = status ? Mem[214] : Mem[3];
status = Mem[110] == 0.0;
Mem[218] = status ? Mem[27] : Mem[216];
status = Mem[114] == 0.0;
Mem[220] = status ? Mem[169] : Mem[218];
status = Mem[118] == 0.0;
Mem[222] = status ? Mem[27] : Mem[220];
status = Mem[31] == 0.0;
Mem[224] = status ? Mem[214] : Mem[222];
status = Mem[35] == 0.0;
Mem[226] = status ? Mem[27] : Mem[224];
status = Mem[40] == 0.0;
Mem[228] = status ? Mem[169] : Mem[226];
status = Mem[44] == 0.0;
Mem[230] = status ? Mem[27] : Mem[228];
status = Mem[5] == 0.0;
Mem[232] = status ? Mem[230] : Mem[213];
Mem[233] = Mem[232] * Mem[84];
Mem[234] = Mem[153] + Mem[233];
Mem[235] = Mem[234] + Mem[212];
Mem[236] = Mem[235] - Mem[207];
Mem[237] = Mem[236] * Mem[236];
Mem[238] = Mem[237] + Mem[189];
Mem[239] = Math.Sqrt(Mem[238]);
Mem[240] = Mem[25] + Mem[239];
Mem[241] = Mem[163] * Mem[163];
Mem[242] = Mem[210] * Mem[210];
Mem[243] = Mem[242] + Mem[241];
Mem[244] = Math.Sqrt(Mem[243]);
Mem[245] = Mem[244] - Mem[3];
status = Mem[245] == 0.0;
Mem[247] = status ? Mem[240] : Mem[3];
Mem[248] = Mem[239] - Mem[17];
status = Mem[248] < 0.0;
Mem[250] = status ? Mem[247] : Mem[3];
Mem[251] = Mem[51] - Mem[102];
Mem[252] = Mem[251] * Mem[251];
Mem[253] = Mem[69] - Mem[207];
Mem[254] = Mem[253] * Mem[253];
Mem[255] = Mem[254] + Mem[252];
Mem[256] = Math.Sqrt(Mem[255]);
Mem[257] = Mem[256] * Mem[256];
Mem[258] = Mem[257] * Mem[256];
Mem[259] = Mem[81] / Mem[258];
Mem[260] = Mem[251] * Mem[259];
Mem[261] = Mem[260] + Mem[83];
Mem[262] = Mem[261] / Mem[15];
Mem[263] = Mem[262] * Mem[84];
Mem[264] = Mem[99] + Mem[263];
Mem[265] = Mem[253] * Mem[259];
Mem[266] = Mem[265] + Mem[190];
Mem[267] = Mem[266] / Mem[15];
Mem[268] = Mem[267] * Mem[84];
Mem[269] = Mem[204] + Mem[268];
Mem[270] = Mem[392];
status = Mem[31] == 0.0;
Mem[273] = status ? Mem[271] : Mem[3];
status = Mem[35] == 0.0;
Mem[275] = status ? Mem[271] : Mem[273];
status = Mem[40] == 0.0;
Mem[277] = status ? Mem[271] : Mem[275];
status = Mem[44] == 0.0;
Mem[279] = status ? Mem[271] : Mem[277];
status = Mem[5] == 0.0;
Mem[281] = status ? Mem[279] : Mem[270];
Mem[282] = Mem[51] - Mem[187];
Mem[283] = Mem[282] * Mem[282];
Mem[284] = Mem[69] - Mem[235];
Mem[285] = Mem[284] * Mem[284];
Mem[286] = Mem[285] + Mem[283];
Mem[287] = Math.Sqrt(Mem[286]);
Mem[288] = Mem[287] * Mem[287];
Mem[289] = Mem[288] * Mem[287];
Mem[290] = Mem[81] / Mem[289];
Mem[291] = Mem[282] * Mem[290];
Mem[292] = Mem[291] + Mem[161];
Mem[293] = Mem[292] / Mem[15];
Mem[294] = Mem[163] + Mem[293];
Mem[295] = Mem[294] * Mem[84];
Mem[296] = Mem[184] + Mem[295];
Mem[297] = Mem[284] * Mem[290];
Mem[298] = Mem[297] + Mem[208];
Mem[299] = Mem[298] / Mem[15];
Mem[300] = Mem[210] + Mem[299];
Mem[301] = Mem[300] * Mem[84];
Mem[302] = Mem[232] + Mem[301];
Mem[303] = Mem[387];
status = Mem[106] == 0.0;
Mem[305] = status ? Mem[271] : Mem[3];
status = Mem[110] == 0.0;
Mem[307] = status ? Mem[271] : Mem[305];
status = Mem[114] == 0.0;
Mem[309] = status ? Mem[271] : Mem[307];
status = Mem[118] == 0.0;
Mem[311] = status ? Mem[271] : Mem[309];
status = Mem[31] == 0.0;
Mem[313] = status ? Mem[271] : Mem[311];
status = Mem[35] == 0.0;
Mem[315] = status ? Mem[271] : Mem[313];
status = Mem[40] == 0.0;
Mem[317] = status ? Mem[271] : Mem[315];
status = Mem[44] == 0.0;
Mem[319] = status ? Mem[271] : Mem[317];
status = Mem[5] == 0.0;
Mem[321] = status ? Mem[319] : Mem[303];
Mem[322] = Mem[384];
status = Mem[5] == 0.0;
Mem[324] = status ? Mem[3] : Mem[322];
Mem[325] = Mem[383];
status = Mem[5] == 0.0;
Mem[327] = status ? Mem[3] : Mem[325];
Mem[328] = Mem[4] + Mem[0];
Mem[329] = Mem[102] - Mem[187];
Mem[330] = Mem[207] - Mem[235];
Mem[331] = Mem[395];
Mem[332] = Mem[331] + Mem[0];
status = Mem[245] == 0.0;
Mem[334] = status ? Mem[332] : Mem[3];
status = Mem[248] < 0.0;
Mem[336] = status ? Mem[334] : Mem[3];
Mem[337] = Mem[397];
status = Mem[5] == 0.0;
Mem[340] = status ? Mem[338] : Mem[337];
Mem[341] = Mem[244] * Mem[84];
Mem[342] = Mem[340] - Mem[341];
Mem[345] = Mem[342] / Mem[338];
Mem[346] = Mem[345] * Mem[344];
Mem[347] = Mem[7] + Mem[346];
Mem[348] = Mem[347] + Mem[343];
Mem[349] = Mem[348] * Mem[15];
Mem[351] = Mem[350] / Mem[84];
Mem[352] = Mem[351] - Mem[336];
status = Mem[352] < 0.0;
Mem[354] = status ? Mem[349] : Mem[3];
Mem[355] = Mem[342] - Mem[3];
Mem[356] = Mem[372] - Mem[0];
status = Mem[355] < 0.0;
Mem[358] = status ? Mem[356] : Mem[354];
Mem[359] = Mem[338] - Mem[341];
status = Mem[359] < 0.0;
Mem[361] = status ? Mem[356] : Mem[358];
Mem[363] = Mem[187] - Mem[51];
Mem[364] = Mem[363] * Mem[363];
Mem[365] = Mem[235] - Mem[69];
Mem[366] = Mem[365] * Mem[365];
Mem[367] = Mem[366] + Mem[364];
Mem[368] = Math.Sqrt(Mem[367]);
Mem[369] = Mem[368] - Mem[362];
status = Mem[369] < 0.0;
Mem[371] = status ? Mem[356] : Mem[361];
Outport[0] = Mem[371];
Outport[1] = Mem[342];
Outport[2] = Mem[284];
Outport[3] = Mem[282];
Outport[4] = Mem[330];
Outport[5] = Mem[329];
Mem[379] = Mem[328];
Mem[380] = Mem[69];
Mem[381] = Mem[51];
Mem[382] = Mem[80];
Mem[383] = Mem[327];
Mem[384] = Mem[324];
Mem[385] = Mem[235];
Mem[386] = Mem[187];
Mem[387] = Mem[321];
Mem[388] = Mem[302];
Mem[389] = Mem[296];
Mem[390] = Mem[207];
Mem[391] = Mem[102];
Mem[392] = Mem[281];
Mem[393] = Mem[269];
Mem[394] = Mem[264];
Mem[395] = Mem[336];
Mem[396] = Mem[250];
Mem[397] = Mem[342];
Mem[398] = Mem[24];
Mem[399] = Mem[22];
Mem[400] = Mem[14];

		}
	}
}