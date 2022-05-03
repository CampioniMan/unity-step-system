using System;
using UnityEngine;
using Random = UnityEngine.Random;
using StepSystem;

namespace UnityEditor.TreeViewExamples
{
	[Serializable]
	internal class StepViewData : TreeElement
	{
		public Texture2D icon;
		public BaseCommonStep step;
		public bool isOptional;
		public string description = "";

		public StepViewData(int depth, int id, BaseCommonStep step, string description, bool isOptional) : base(step?.name ?? "", depth, id)
		{
			this.step = step;
			this.description = description;
			this.isOptional = isOptional;
		}
	}
}
