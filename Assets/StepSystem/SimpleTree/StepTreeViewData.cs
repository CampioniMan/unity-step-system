using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace StepSystem.SimpleTree {
	[CreateAssetMenu(fileName = "TreeDataAsset", menuName = "Tree Asset", order = 1)]
	public class StepTreeViewData : ScriptableObject {
		[FormerlySerializedAs("m_TreeElements")] [SerializeField] List<StepViewData> treeElements = new List<StepViewData>();
		TreeModel<StepViewData> _stepTree;

		public List<StepViewData> TreeElements => treeElements;

		public void Prepare() {
			_stepTree = new TreeModel<StepViewData>(treeElements);
		}

		void PrepareChildren(TreeElement element) {
			if (element?.Children == null) {
				return;
			}

			foreach (TreeElement treeElement in element.Children) {
				var child = (StepViewData)treeElement;
				child.step.Prepare();
				PrepareChildren(child);
			}
		}

		public void Execute() {
			ExecuteChildren(_stepTree.Root);
		}

		static void ExecuteChildren(StepViewData element) {
			if (element?.Children == null) {
				//TODO: Notify progress
				return;
			}

			foreach (TreeElement treeElement in element.Children) {
				var child = (StepViewData)treeElement;
				child.step.Execute((success) => {
					if (success) {
						ExecuteChildren(child);
					}
				});
			}
		}
	}
}