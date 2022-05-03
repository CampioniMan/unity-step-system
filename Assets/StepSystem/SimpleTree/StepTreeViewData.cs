using System.Collections.Generic;
using UnityEngine;

namespace StepSystem.SimpleTree
{
	[CreateAssetMenu(fileName = "TreeDataAsset", menuName = "Tree Asset", order = 1)]
	public class StepTreeViewData : ScriptableObject
	{
		[SerializeField] List<StepViewData> m_TreeElements = new List<StepViewData>();
		TreeModel<StepViewData> stepTree;

		public List<StepViewData> treeElements
		{
			get { return m_TreeElements; }
			set { m_TreeElements = value; }
		}

		public void Prepare()
		{
			stepTree = new TreeModel<StepViewData>(m_TreeElements);
		}

		void PrepareChildren(StepViewData element)
		{
			if (element == null || element.children == null)
			{
				return;
			}
			foreach (StepViewData child in element.children)
			{
				child.step.Prepare();
				PrepareChildren(child);
			}
		}

		public void Execute()
		{
			ExecuteChildren(stepTree.root);
		}

		void ExecuteChildren(StepViewData element)
		{
			if (element == null || element.children == null)
			{
				//TODO: Notify progress
				return;
			}

			foreach (StepViewData child in element.children)
			{
				child.step.Execute((success) =>
				{
					if (success)
					{
						ExecuteChildren(child);
					}
				});
			}
		}
	}
}
