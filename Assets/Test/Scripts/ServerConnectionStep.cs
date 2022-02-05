using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StepSystem;

namespace Tests
{
	[CreateAssetMenu(fileName = "ServerConnectionStep", menuName = "Tree Asset Tests/Step 01", order = 1)]
	public class ServerConnectionStep : BaseAsyncStep
	{
		public override void Prepare()
		{
			Debug.Log("Preparing server connection...");
		}

		protected override Task<bool> ExecuteTask()
		{
			// It's for testing purposes
			return Task<bool>.Run(async delegate
			{
				await Task.Delay(1500);
				Debug.Log("Connected to the server!");
				return true;
			});
		}

		public override void Dispose()
		{
			Debug.Log("Disposing server step");
		}
	}
}
