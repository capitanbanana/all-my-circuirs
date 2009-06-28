namespace ifpfc.Logic
{
	public class LogicState
	{
		public double Score { get; set; }
		public double Fuel { get; set; }

		public double Sx
		{
			get { return S.x; }
		}

		public double Sy
		{
			get { return S.y; }
		}

		public Vector S { get; set; }

		public double Vx
		{
			get { return V.x; }
		}

		public double Vy
		{
			get { return V.y; }
		}

		public Vector V { get; set; }
	}
}