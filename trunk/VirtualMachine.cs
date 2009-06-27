using System;

namespace ifpfc
{
	public class VirtualMachine : IVirtualMachine
	{
		public VirtualMachine(int teamId, int scenarioId)
		{
		}

		public double[] RunTimeStep(double dx, double dy)
		{
			throw new NotImplementedException();
		}

		public byte[] FormSubmission()
		{
			throw new NotImplementedException();
		}
	}
}