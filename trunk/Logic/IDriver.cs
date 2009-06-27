namespace ifpfc.Logic
{
	public interface IDriver
	{
		bool IsEnd();
		Vector RunStep(double[] outPorts);
	}
}