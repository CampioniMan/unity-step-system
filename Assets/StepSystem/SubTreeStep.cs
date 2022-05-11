using System;
using UnityEngine;
using StepSystem.SimpleTree;
using UnityEngine.Serialization;

namespace StepSystem {
	public class SubTreeStep : BaseCommonStep {
		[FormerlySerializedAs("_subTree")] [SerializeField]
		StepTreeViewData subTree;

		public override void Prepare() {
			subTree.Prepare();
		}

		public override void Execute(Action<bool> onFinish) {
			subTree.Execute();
		}

		public override void Dispose() { }
	}
}