using System;
using System.Threading.Tasks;

namespace StepSystem {
	public abstract class BaseAsyncStep : BaseCommonStep {
		protected abstract Task<bool> ExecuteTask();

		public override void Execute(Action<bool> onFinish) {
			Task.Run(ExecuteTask).ContinueWith((executedTask) => { onFinish.Invoke(executedTask.Result); });
		}

		//TODO: Add the possibility of canceling an async step
		//public void CancelExecution();
	}
}