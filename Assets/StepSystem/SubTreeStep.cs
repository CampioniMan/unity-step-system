using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StepSystem.SimpleTree;

namespace StepSystem
{
	public class SubTreeStep : BaseCommonStep
	{
		[SerializeField] StepTreeViewData _subTree;

		public override void Prepare()
		{
			_subTree.Prepare();
		}

		public override void Execute(Action<bool> onFinish)
		{
			_subTree.Execute();
		}

		public override void Dispose() {}
	}
}
