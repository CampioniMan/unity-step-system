using System;
using UnityEngine;

namespace StepSystem.SimpleTree {
	[Serializable]
	public class StepViewData : TreeElement {
		public Texture2D icon;
		public BaseCommonStep step;
		public bool isOptional;
		public string description;

		public StepViewData(int depth, int id, BaseCommonStep step, string description, bool isOptional)
			: base(step != null ? step.name : "", depth, id) {
			this.step = step;
			this.description = description;
			this.isOptional = isOptional;
		}
	}
}