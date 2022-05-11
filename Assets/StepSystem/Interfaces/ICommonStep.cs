using System;

namespace StepSystem.Interfaces {
	public interface ICommonStep : IBaseStep {
		void Execute(Action<bool> onFinish);
	}
}