namespace ifpfc.Logic
{
	public class Sattelite
	{
		public Sattelite(string name, Vector location, Vector speed)
		{
			Name = name;
			Location = location;
			Speed = speed;
		}

		public readonly string Name;
		public readonly Vector Location;
		public readonly Vector Speed;
	}
}