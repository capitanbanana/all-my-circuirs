using System;
using System.Collections.Generic;
using System.Linq;
using ifpfc.Logic;

namespace ifpfc.VM
{
	public class EccentricMeetAndGreetEngine : IVirtualMachine
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
		

		public EccentricMeetAndGreetEngine(int teamId, int scenarioId)
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
Mem[15] = BitConverter.Int64BitsToDouble(4611686018427387904);
Mem[17] = BitConverter.Int64BitsToDouble(4652007308841189376);
Mem[28] = BitConverter.Int64BitsToDouble(4710731271397965824);
Mem[29] = BitConverter.Int64BitsToDouble(4658824280933400576);
Mem[34] = BitConverter.Int64BitsToDouble(4658822081910145024);
Mem[38] = BitConverter.Int64BitsToDouble(-4514788249104809984);
Mem[39] = BitConverter.Int64BitsToDouble(4658819882886889472);
Mem[43] = BitConverter.Int64BitsToDouble(4658817683863633920);
Mem[57] = BitConverter.Int64BitsToDouble(4709657529573965824);
Mem[60] = BitConverter.Int64BitsToDouble(4723801030825869312);
Mem[76] = BitConverter.Int64BitsToDouble(4454720405870426065);
Mem[78] = BitConverter.Int64BitsToDouble(4977561924064720455);
Mem[89] = BitConverter.Int64BitsToDouble(-4556468031118256774);
Mem[92] = BitConverter.Int64BitsToDouble(-4566223211822362831);
Mem[95] = BitConverter.Int64BitsToDouble(-4558277715113817149);
Mem[104] = BitConverter.Int64BitsToDouble(4708691161932365824);
Mem[105] = BitConverter.Int64BitsToDouble(4656528500654604288);
Mem[110] = BitConverter.Int64BitsToDouble(4656524102608093184);
Mem[115] = BitConverter.Int64BitsToDouble(4656519704561582080);
Mem[120] = BitConverter.Int64BitsToDouble(4656515306515070976);
Mem[146] = BitConverter.Int64BitsToDouble(4708583787749965824);
Mem[149] = BitConverter.Int64BitsToDouble(4708798536114765824);
Mem[175] = BitConverter.Int64BitsToDouble(-4559038833935623131);
Mem[178] = BitConverter.Int64BitsToDouble(-4561461077237492124);
Mem[181] = BitConverter.Int64BitsToDouble(-4558057412737400149);
Mem[202] = BitConverter.Int64BitsToDouble(4665094321740958659);
Mem[207] = BitConverter.Int64BitsToDouble(-4581279364356630449);
Mem[223] = BitConverter.Int64BitsToDouble(4665380905641176256);
Mem[280] = BitConverter.Int64BitsToDouble(4607182418800017408);
Mem[347] = BitConverter.Int64BitsToDouble(4677104761256804352);
Mem[352] = BitConverter.Int64BitsToDouble(4616189618054758400);
Mem[353] = BitConverter.Int64BitsToDouble(4627730092099895296);
Mem[354] = BitConverter.Int64BitsToDouble(4631530004285489152);
Mem[360] = BitConverter.Int64BitsToDouble(4651127699538968576);
Mem[372] = BitConverter.Int64BitsToDouble(4708583787749965824);

		}

		private void Execute()
		{
			//Generated code
			Mem[1] = Mem[410];
Mem[4] = Mem[389];
Mem[5] = Mem[4] - Mem[3];
status = Mem[5] == 0.0;
Mem[7] = status ? Mem[2] : Mem[1];
Mem[8] = Mem[7] - Mem[0];
Mem[9] = Mem[408];
status = Mem[5] == 0.0;
Mem[11] = status ? Mem[0] : Mem[9];
Mem[12] = Mem[11] - Mem[0];
status = Mem[12] == 0.0;
Mem[14] = status ? Mem[8] : Mem[7];
Mem[16] = Mem[409];
status = Mem[5] == 0.0;
Mem[19] = status ? Mem[17] : Mem[16];
Mem[20] = Mem[19] * Mem[15];
status = Mem[12] == 0.0;
Mem[22] = status ? Mem[20] : Mem[19];
status = Mem[12] == 0.0;
Mem[24] = status ? Mem[19] : Mem[12];
Mem[25] = Mem[406];
Mem[26] = Mem[401];
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
Mem[49] = Mem[391];
status = Mem[5] == 0.0;
Mem[51] = status ? Mem[3] : Mem[49];
Mem[52] = Mem[51] - Mem[48];
Mem[53] = Mem[52] * Mem[52];
Mem[54] = Mem[400];
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
Mem[67] = Mem[390];
status = Mem[5] == 0.0;
Mem[69] = status ? Mem[3] : Mem[67];
Mem[70] = Mem[69] - Mem[66];
Mem[71] = Mem[70] * Mem[70];
Mem[72] = Mem[71] + Mem[53];
Mem[73] = Math.Sqrt(Mem[72]);
Mem[74] = Mem[73] * Mem[73];
Mem[75] = Mem[74] * Mem[73];
Mem[77] = Mem[392];
status = Mem[5] == 0.0;
Mem[80] = status ? Mem[78] : Mem[77];
Mem[81] = Mem[76] * Mem[80];
Mem[82] = (Mem[75] == 0) ? 0.0 : (Mem[81] / Mem[75]);
Mem[83] = Mem[52] * Mem[82];
Mem[84] = (Mem[0] == 0) ? 0.0 : (Mem[0] / Mem[0]);
Mem[85] = Mem[84] * Mem[84];
Mem[86] = (Mem[15] == 0) ? 0.0 : (Mem[85] / Mem[15]);
Mem[87] = Mem[83] * Mem[86];
Mem[88] = Mem[404];
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
Mem[103] = Mem[396];
Mem[106] = Mem[105] + Mem[17];
Mem[107] = Mem[30] - Mem[106];
status = Mem[107] == 0.0;
Mem[109] = status ? Mem[104] : Mem[3];
Mem[111] = Mem[110] + Mem[17];
Mem[112] = Mem[30] - Mem[111];
status = Mem[112] == 0.0;
Mem[114] = status ? Mem[27] : Mem[109];
Mem[116] = Mem[115] + Mem[17];
Mem[117] = Mem[30] - Mem[116];
status = Mem[117] == 0.0;
Mem[119] = status ? Mem[38] : Mem[114];
Mem[121] = Mem[120] + Mem[17];
Mem[122] = Mem[30] - Mem[121];
status = Mem[122] == 0.0;
Mem[124] = status ? Mem[27] : Mem[119];
Mem[125] = Mem[30] - Mem[105];
status = Mem[125] == 0.0;
Mem[127] = status ? Mem[104] : Mem[124];
Mem[128] = Mem[30] - Mem[110];
status = Mem[128] == 0.0;
Mem[130] = status ? Mem[27] : Mem[127];
Mem[131] = Mem[30] - Mem[115];
status = Mem[131] == 0.0;
Mem[133] = status ? Mem[38] : Mem[130];
Mem[134] = Mem[30] - Mem[120];
status = Mem[134] == 0.0;
Mem[136] = status ? Mem[27] : Mem[133];
status = Mem[5] == 0.0;
Mem[138] = status ? Mem[136] : Mem[103];
Mem[139] = Mem[51] - Mem[138];
Mem[140] = Mem[139] * Mem[139];
Mem[141] = Mem[395];
status = Mem[107] == 0.0;
Mem[143] = status ? Mem[27] : Mem[3];
status = Mem[112] == 0.0;
Mem[145] = status ? Mem[28] : Mem[143];
status = Mem[117] == 0.0;
Mem[148] = status ? Mem[146] : Mem[145];
status = Mem[122] == 0.0;
Mem[151] = status ? Mem[149] : Mem[148];
status = Mem[125] == 0.0;
Mem[153] = status ? Mem[27] : Mem[151];
status = Mem[128] == 0.0;
Mem[155] = status ? Mem[28] : Mem[153];
status = Mem[131] == 0.0;
Mem[157] = status ? Mem[146] : Mem[155];
status = Mem[134] == 0.0;
Mem[159] = status ? Mem[149] : Mem[157];
status = Mem[5] == 0.0;
Mem[161] = status ? Mem[159] : Mem[141];
Mem[162] = Mem[69] - Mem[161];
Mem[163] = Mem[162] * Mem[162];
Mem[164] = Mem[163] + Mem[140];
Mem[165] = Math.Sqrt(Mem[164]);
Mem[166] = Mem[165] * Mem[165];
Mem[167] = Mem[166] * Mem[165];
Mem[168] = (Mem[167] == 0) ? 0.0 : (Mem[81] / Mem[167]);
Mem[169] = Mem[139] * Mem[168];
Mem[170] = Inport[3];
Mem[171] = (Mem[84] == 0) ? 0.0 : (Mem[170] / Mem[84]);
Mem[172] = Mem[171] + Mem[169];
Mem[173] = Mem[172] * Mem[86];
Mem[174] = Mem[399];
status = Mem[112] == 0.0;
Mem[177] = status ? Mem[175] : Mem[143];
status = Mem[117] == 0.0;
Mem[180] = status ? Mem[178] : Mem[177];
status = Mem[122] == 0.0;
Mem[183] = status ? Mem[181] : Mem[180];
status = Mem[125] == 0.0;
Mem[185] = status ? Mem[27] : Mem[183];
status = Mem[128] == 0.0;
Mem[187] = status ? Mem[175] : Mem[185];
status = Mem[131] == 0.0;
Mem[189] = status ? Mem[178] : Mem[187];
status = Mem[134] == 0.0;
Mem[191] = status ? Mem[181] : Mem[189];
status = Mem[5] == 0.0;
Mem[193] = status ? Mem[191] : Mem[174];
Mem[194] = Mem[193] * Mem[84];
Mem[195] = Mem[138] + Mem[194];
Mem[196] = Mem[195] + Mem[173];
Mem[197] = Mem[196] - Mem[102];
Mem[198] = Mem[197] * Mem[197];
Mem[199] = Mem[70] * Mem[82];
Mem[200] = Mem[199] * Mem[86];
Mem[201] = Mem[403];
status = Mem[31] == 0.0;
Mem[204] = status ? Mem[202] : Mem[3];
status = Mem[35] == 0.0;
Mem[206] = status ? Mem[27] : Mem[204];
status = Mem[40] == 0.0;
Mem[209] = status ? Mem[207] : Mem[206];
status = Mem[44] == 0.0;
Mem[211] = status ? Mem[27] : Mem[209];
status = Mem[5] == 0.0;
Mem[213] = status ? Mem[211] : Mem[201];
Mem[214] = Mem[213] * Mem[84];
Mem[215] = Mem[66] + Mem[214];
Mem[216] = Mem[215] + Mem[200];
Mem[217] = Mem[162] * Mem[168];
Mem[218] = Inport[2];
Mem[219] = (Mem[84] == 0) ? 0.0 : (Mem[218] / Mem[84]);
Mem[220] = Mem[219] + Mem[217];
Mem[221] = Mem[220] * Mem[86];
Mem[222] = Mem[398];
status = Mem[107] == 0.0;
Mem[225] = status ? Mem[223] : Mem[3];
status = Mem[112] == 0.0;
Mem[227] = status ? Mem[27] : Mem[225];
status = Mem[117] == 0.0;
Mem[229] = status ? Mem[178] : Mem[227];
status = Mem[122] == 0.0;
Mem[231] = status ? Mem[27] : Mem[229];
status = Mem[125] == 0.0;
Mem[233] = status ? Mem[223] : Mem[231];
status = Mem[128] == 0.0;
Mem[235] = status ? Mem[27] : Mem[233];
status = Mem[131] == 0.0;
Mem[237] = status ? Mem[178] : Mem[235];
status = Mem[134] == 0.0;
Mem[239] = status ? Mem[27] : Mem[237];
status = Mem[5] == 0.0;
Mem[241] = status ? Mem[239] : Mem[222];
Mem[242] = Mem[241] * Mem[84];
Mem[243] = Mem[161] + Mem[242];
Mem[244] = Mem[243] + Mem[221];
Mem[245] = Mem[244] - Mem[216];
Mem[246] = Mem[245] * Mem[245];
Mem[247] = Mem[246] + Mem[198];
Mem[248] = Math.Sqrt(Mem[247]);
Mem[249] = Mem[25] + Mem[248];
Mem[250] = Mem[171] * Mem[171];
Mem[251] = Mem[219] * Mem[219];
Mem[252] = Mem[251] + Mem[250];
Mem[253] = Math.Sqrt(Mem[252]);
Mem[254] = Mem[253] - Mem[3];
status = Mem[254] == 0.0;
Mem[256] = status ? Mem[249] : Mem[3];
Mem[257] = Mem[248] - Mem[17];
status = Mem[257] < 0.0;
Mem[259] = status ? Mem[256] : Mem[3];
Mem[260] = Mem[51] - Mem[102];
Mem[261] = Mem[260] * Mem[260];
Mem[262] = Mem[69] - Mem[216];
Mem[263] = Mem[262] * Mem[262];
Mem[264] = Mem[263] + Mem[261];
Mem[265] = Math.Sqrt(Mem[264]);
Mem[266] = Mem[265] * Mem[265];
Mem[267] = Mem[266] * Mem[265];
Mem[268] = (Mem[267] == 0) ? 0.0 : (Mem[81] / Mem[267]);
Mem[269] = Mem[260] * Mem[268];
Mem[270] = Mem[269] + Mem[83];
Mem[271] = (Mem[15] == 0) ? 0.0 : (Mem[270] / Mem[15]);
Mem[272] = Mem[271] * Mem[84];
Mem[273] = Mem[99] + Mem[272];
Mem[274] = Mem[262] * Mem[268];
Mem[275] = Mem[274] + Mem[199];
Mem[276] = (Mem[15] == 0) ? 0.0 : (Mem[275] / Mem[15]);
Mem[277] = Mem[276] * Mem[84];
Mem[278] = Mem[213] + Mem[277];
Mem[279] = Mem[402];
status = Mem[31] == 0.0;
Mem[282] = status ? Mem[280] : Mem[3];
status = Mem[35] == 0.0;
Mem[284] = status ? Mem[280] : Mem[282];
status = Mem[40] == 0.0;
Mem[286] = status ? Mem[280] : Mem[284];
status = Mem[44] == 0.0;
Mem[288] = status ? Mem[280] : Mem[286];
status = Mem[5] == 0.0;
Mem[290] = status ? Mem[288] : Mem[279];
Mem[291] = Mem[51] - Mem[196];
Mem[292] = Mem[291] * Mem[291];
Mem[293] = Mem[69] - Mem[244];
Mem[294] = Mem[293] * Mem[293];
Mem[295] = Mem[294] + Mem[292];
Mem[296] = Math.Sqrt(Mem[295]);
Mem[297] = Mem[296] * Mem[296];
Mem[298] = Mem[297] * Mem[296];
Mem[299] = (Mem[298] == 0) ? 0.0 : (Mem[81] / Mem[298]);
Mem[300] = Mem[291] * Mem[299];
Mem[301] = Mem[300] + Mem[169];
Mem[302] = (Mem[15] == 0) ? 0.0 : (Mem[301] / Mem[15]);
Mem[303] = Mem[171] + Mem[302];
Mem[304] = Mem[303] * Mem[84];
Mem[305] = Mem[193] + Mem[304];
Mem[306] = Mem[293] * Mem[299];
Mem[307] = Mem[306] + Mem[217];
Mem[308] = (Mem[15] == 0) ? 0.0 : (Mem[307] / Mem[15]);
Mem[309] = Mem[219] + Mem[308];
Mem[310] = Mem[309] * Mem[84];
Mem[311] = Mem[241] + Mem[310];
Mem[312] = Mem[397];
status = Mem[107] == 0.0;
Mem[314] = status ? Mem[280] : Mem[3];
status = Mem[112] == 0.0;
Mem[316] = status ? Mem[280] : Mem[314];
status = Mem[117] == 0.0;
Mem[318] = status ? Mem[280] : Mem[316];
status = Mem[122] == 0.0;
Mem[320] = status ? Mem[280] : Mem[318];
status = Mem[125] == 0.0;
Mem[322] = status ? Mem[280] : Mem[320];
status = Mem[128] == 0.0;
Mem[324] = status ? Mem[280] : Mem[322];
status = Mem[131] == 0.0;
Mem[326] = status ? Mem[280] : Mem[324];
status = Mem[134] == 0.0;
Mem[328] = status ? Mem[280] : Mem[326];
status = Mem[5] == 0.0;
Mem[330] = status ? Mem[328] : Mem[312];
Mem[331] = Mem[394];
status = Mem[5] == 0.0;
Mem[333] = status ? Mem[3] : Mem[331];
Mem[334] = Mem[393];
status = Mem[5] == 0.0;
Mem[336] = status ? Mem[3] : Mem[334];
Mem[337] = Mem[4] + Mem[0];
Mem[338] = Mem[102] - Mem[196];
Mem[339] = Mem[216] - Mem[244];
Mem[340] = Mem[405];
Mem[341] = Mem[340] + Mem[0];
status = Mem[254] == 0.0;
Mem[343] = status ? Mem[341] : Mem[3];
status = Mem[257] < 0.0;
Mem[345] = status ? Mem[343] : Mem[3];
Mem[346] = Mem[407];
status = Mem[5] == 0.0;
Mem[349] = status ? Mem[347] : Mem[346];
Mem[350] = Mem[253] * Mem[84];
Mem[351] = Mem[349] - Mem[350];
Mem[355] = (Mem[347] == 0) ? 0.0 : (Mem[351] / Mem[347]);
Mem[356] = Mem[355] * Mem[354];
Mem[357] = Mem[7] + Mem[356];
Mem[358] = Mem[357] + Mem[353];
Mem[359] = Mem[358] * Mem[352];
Mem[361] = (Mem[84] == 0) ? 0.0 : (Mem[360] / Mem[84]);
Mem[362] = Mem[361] - Mem[345];
status = Mem[362] < 0.0;
Mem[364] = status ? Mem[359] : Mem[3];
Mem[365] = Mem[351] - Mem[3];
Mem[366] = Mem[382] - Mem[0];
status = Mem[365] < 0.0;
Mem[368] = status ? Mem[366] : Mem[364];
Mem[369] = Mem[347] - Mem[350];
status = Mem[369] < 0.0;
Mem[371] = status ? Mem[366] : Mem[368];
Mem[373] = Mem[196] - Mem[51];
Mem[374] = Mem[373] * Mem[373];
Mem[375] = Mem[244] - Mem[69];
Mem[376] = Mem[375] * Mem[375];
Mem[377] = Mem[376] + Mem[374];
Mem[378] = Math.Sqrt(Mem[377]);
Mem[379] = Mem[378] - Mem[372];
status = Mem[379] < 0.0;
Mem[381] = status ? Mem[366] : Mem[371];
Outport[0] = Mem[381];
Outport[1] = Mem[351];
Outport[2] = Mem[293];
Outport[3] = Mem[291];
Outport[4] = Mem[339];
Outport[5] = Mem[338];
Mem[389] = Mem[337];
Mem[390] = Mem[69];
Mem[391] = Mem[51];
Mem[392] = Mem[80];
Mem[393] = Mem[336];
Mem[394] = Mem[333];
Mem[395] = Mem[244];
Mem[396] = Mem[196];
Mem[397] = Mem[330];
Mem[398] = Mem[311];
Mem[399] = Mem[305];
Mem[400] = Mem[216];
Mem[401] = Mem[102];
Mem[402] = Mem[290];
Mem[403] = Mem[278];
Mem[404] = Mem[273];
Mem[405] = Mem[345];
Mem[406] = Mem[259];
Mem[407] = Mem[351];
Mem[408] = Mem[24];
Mem[409] = Mem[22];
Mem[410] = Mem[14];

		}
	}
}