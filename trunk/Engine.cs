namespace ifpfc
{
	public class Engine
	{
		private const int addressSize = 14;
		private const int addressSpaceSize = 1 << addressSize;
		private bool status;

		public double[] Outport { get; private set; }
		public double[] Inport { get; private set; }
		public double[] Mem { get; private set; }
		public Engine(int configNumber)
		{
			Mem = new double[addressSpaceSize];
			Inport = new double[addressSpaceSize];
			Outport = new double[addressSpaceSize];
			Inport[16000] = configNumber;
			InitMem();
		}

		private void InitMem()
		{
			//Generated code
		}

		public void Step(double dx, double dy)
		{
			Inport[2] = dx;
			Inport[3] = dy;
			Execute();
		}

		private void Execute()
		{
			//Generated code
		}
	}
}