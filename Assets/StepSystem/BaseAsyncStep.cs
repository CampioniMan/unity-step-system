using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using StepSystem.Interfaces;

namespace StepSystem
{
	public abstract class BaseAsyncStep : BaseCommonStep, IAsyncStep
	{
		CancellationTokenSource source;
		CancellationToken token;

		public abstract Task<bool> ExecuteTask();

		public override void Execute(Action<bool> onFinish)
		{
			source = new CancellationTokenSource();
			token = source.Token;
			Task<bool>.Run(ExecuteTask, token).ContinueWith((executedTask) => 
			{
				onFinish.Invoke(executedTask.Result);
			}, token);
		}

		public void CancelExecution()
		{
			source.Cancel();
		}
	}
}
