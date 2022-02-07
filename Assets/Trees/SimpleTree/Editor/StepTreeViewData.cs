using System;
using UnityEngine;
using Random = UnityEngine.Random;
using StepSystem;

namespace UnityEditor.TreeViewExamples
{
	[Serializable]
	internal class StepTreeViewData : TreeElement
	{
		public bool isOptional;
		public int weight = 1;
		public BaseCommonStep step;
		public string description = "";
		public Texture2D icon;

		public StepTreeViewData(int depth, int id, BaseCommonStep step, string description, bool isOptional, int weight = 1) : base(step?.name ?? "", depth, id)
		{
			this.step = step;
			this.description = description;
			this.isOptional = isOptional;
			this.weight = weight;
		}
	}
}
