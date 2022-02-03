using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityEditor.TreeViewExamples
{
	static class StepTreeViewDataGenerator
	{
		static int IDCounter;
		static int minNumChildren = 5;
		static int maxNumChildren = 10;
		static float probabilityOfBeingLeaf = 0.5f;

		public static List<StepTreeViewData> GenerateRandomTree(int numTotalElements)
		{
			int numRootChildren = numTotalElements / 4;
			IDCounter = 0;
			var treeElements = new List<StepTreeViewData>(numTotalElements);

			var root = new StepTreeViewData(-1, IDCounter, null, "", false, 1);
			treeElements.Add(root);
			for (int i = 0; i < numRootChildren; ++i)
			{
				int allowedDepth = 6;
				AddChildrenRecursive(root, Random.Range(minNumChildren, maxNumChildren), true, numTotalElements, ref allowedDepth, treeElements);
			}

			return treeElements;
		}
		static void AddChildrenRecursive(TreeElement element, int numChildren, bool force, int numTotalElements, ref int allowedDepth, List<StepTreeViewData> treeElements)
		{
			if (element.depth >= allowedDepth)
			{
				allowedDepth = 0;
				return;
			}

			for (int i = 0; i < numChildren; ++i)
			{
				if (IDCounter > numTotalElements)
					return;

				var child = new StepTreeViewData(element.depth + 1, ++IDCounter, null, "", false, 1);
				treeElements.Add(child);

				if (!force && Random.value < probabilityOfBeingLeaf)
					continue;

				AddChildrenRecursive(child, Random.Range(minNumChildren, maxNumChildren), false, numTotalElements, ref allowedDepth, treeElements);
			}
		}
	}
}
