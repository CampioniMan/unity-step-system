using System.Threading.Tasks;
using StepSystem;
using UnityEngine;

namespace Test.Scripts {
	[CreateAssetMenu(fileName = "ServerConnectionStep", menuName = "Tree Asset Tests/Step 01", order = 1)]
	public class ServerConnectionStep : BaseAsyncStep {
		public override void Prepare() {
			Debug.Log("Preparing server connection...");
		}

		protected override Task<bool> ExecuteTask() {
			// It's for testing purposes
			return Task.Run(async delegate {
				await Task.Delay(1500);
				Debug.Log("Connected to the server!");
				return true;
			});
		}

		public override void Dispose() {
			Debug.Log("Disposing server step");
		}
	}
}
