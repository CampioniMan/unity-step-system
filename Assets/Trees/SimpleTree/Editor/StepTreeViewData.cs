using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityEditor.TreeViewExamples
{
	[Serializable]
	internal class StepTreeViewData : TreeElement
	{
		public bool isOptional;
		public int weight = 1;
		public ScriptableObject step;
		public string description = "";

		//EditorGUIUtility.FindTexture ("Folder Icon"),
		public string icon = "";

		public StepTreeViewData(int depth, int id, ScriptableObject step, string description, bool isOptional, int weight = 1) : base(step.name ?? "", depth, id)
		{
			this.step = step;
			this.description = description;
			this.isOptional = isOptional;
			this.weight = weight;
		}
	}
}
