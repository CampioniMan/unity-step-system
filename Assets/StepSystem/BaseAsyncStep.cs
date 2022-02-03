using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using StepSystem.Interfaces;

namespace StepSystem
{
	public abstract class BaseAsyncStep : BaseCommonStep
	{
		protected abstract Task<bool> ExecuteTask();

		public override void Execute(Action<bool> onFinish)
		{
			Task<bool>.Run(ExecuteTask).ContinueWith((executedTask) => 
			{
				onFinish.Invoke(executedTask.Result);
			});
		}

		//TODO: Add the possibility of canceling an async step
		//public void CancelExecution();
	}
}
