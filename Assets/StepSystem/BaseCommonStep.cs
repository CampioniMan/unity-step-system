using System;
using UnityEngine;

namespace StepSystem {
	public abstract class BaseCommonStep : ScriptableObject {
		public abstract void Prepare();
		public abstract void Execute(Action<bool> onFinish);
		public abstract void Dispose();
	}
}