using SKBKontur.LIT.Core;

namespace ifpfc
{
	public static class TestData
	{
		private static Directory VMImagesDirectory
		{
			get
			{
				Directory result = Directory.CurrentDirectory;
				while (result.GetFiles("*.csproj").Length == 0)
					result = result.Parent;
				return result.GetDirectory("ProblemVMImages");
			}
		}

		public static byte[] Hohmann
		{
			get { return ReadImage("bin1.obf"); }
		}

		public static byte[] ReadImage(string imageName)
		{
			return VMImagesDirectory.GetFile(imageName).Content.Data;
		}
	}
}