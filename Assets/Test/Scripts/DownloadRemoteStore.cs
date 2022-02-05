using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StepSystem;

namespace Tests
{
	[CreateAssetMenu(fileName = "DownloadRemoteStore", menuName = "Tree Asset Tests/Step 02", order = 1)]
	public class DownloadRemoteStore : BaseAsyncStep
	{
		public override void Prepare()
		{
			Debug.Log("Preparing the download of the remote store...");
		}

		protected override Task<bool> ExecuteTask()
		{
			// It's for testing purposes
			return Task<bool>.Run(async delegate
			{
				await Task.Delay(500);
				Debug.Log("Store downloaded!");
				return true;
			});
		}

		public override void Dispose()
		{
			Debug.Log("Disposing store download");
		}
	}
}
