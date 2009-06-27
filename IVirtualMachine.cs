namespace ifpfc
{
	public interface IVirtualMachine
	{
		double[] RunTimeStep(double dx, double dy);
		byte[] CreateSubmission();
	}
}