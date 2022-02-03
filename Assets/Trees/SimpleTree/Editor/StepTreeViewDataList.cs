using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.TreeViewExamples
{
	[CreateAssetMenu(fileName = "TreeDataAsset", menuName = "Tree Asset", order = 1)]
	public class StepTreeViewDataList : ScriptableObject
	{
		[SerializeField] List<StepTreeViewData> m_TreeElements = new List<StepTreeViewData>();

		internal List<StepTreeViewData> treeElements
		{
			get { return m_TreeElements; }
			set { m_TreeElements = value; }
		}

		void Awake()
		{
			if (m_TreeElements.Count == 0)
			{
				m_TreeElements = StepTreeViewDataGenerator.GenerateRandomTree(40);
			}
		}
	}
}
