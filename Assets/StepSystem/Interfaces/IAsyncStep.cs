using System.Threading.Tasks;

namespace StepSystem.Interfaces {
	public interface IAsyncStep : IBaseStep {
		Task<bool> ExecuteTask();
	}
}