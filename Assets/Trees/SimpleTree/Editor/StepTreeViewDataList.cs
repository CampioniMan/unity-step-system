using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.TreeViewExamples
{
	[CreateAssetMenu(fileName = "TreeDataAsset", menuName = "Tree Asset", order = 1)]
	public class StepTreeViewDataList : ScriptableObject
	{
		[SerializeField] List<StepTreeViewData> m_TreeElements = new List<StepTreeViewData>();
		TreeModel<StepTreeViewData> stepTree;

		internal List<StepTreeViewData> treeElements
		{
			get { return m_TreeElements; }
			set { m_TreeElements = value; }
		}

		public void Preprare()
		{
			stepTree = new TreeModel<StepTreeViewData>(m_TreeElements);
		}

		void PrepareChildren(StepTreeViewData element)
		{
			if (element == null || element.children == null)
			{
				return;
			}
			foreach (StepTreeViewData child in element.children)
			{
				child.step.Prepare();
				PrepareChildren(child);
			}
		}

		public void Execute()
		{
			ExecuteChildren(stepTree.root);
		}

		void ExecuteChildren(StepTreeViewData element)
		{
			if (element == null || element.children == null)
			{
				//TODO: Notify progress
				return;
			}

			foreach (StepTreeViewData child in element.children)
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
