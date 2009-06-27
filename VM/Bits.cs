namespace ifpfc
{
	internal static class Bits
	{
		public static uint ExtractBits(this uint x, int start, int count)
		{
			return (x >> start) & ((1U << count) - 1);
		}
	}
}