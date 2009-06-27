namespace ifpfc.Logic
{
	public struct Vector
	{
		public double x, y;

		public Vector(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		public static readonly Vector Zero = new Vector(0.0, 0.0);
	}
}