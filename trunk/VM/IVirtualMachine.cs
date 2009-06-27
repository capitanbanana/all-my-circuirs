using ifpfc.Logic;

namespace ifpfc.VM
{
	public interface IVirtualMachine
	{
		double[] RunTimeStep(Vector dv);
		byte[] CreateSubmission();
	}
}