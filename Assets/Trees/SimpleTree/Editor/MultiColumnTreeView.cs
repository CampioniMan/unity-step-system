using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;
using StepSystem;

namespace UnityEditor.TreeViewExamples
{
	internal class MultiColumnTreeView : TreeViewWithTreeModel<StepTreeViewData>
	{
		const float kRowHeights = 20f;
		const float kToggleWidth = 18f;

		public StepTreeViewData CurrentSelectedElement { get; private set; }

		// All columns
		enum MyColumns
		{
			Icon,
			StepAsset,
			IsOptional,
			Weight,
			Description
		}

		public enum SortOption
		{
			StepAsset
		}

		// Sort options per column
		SortOption[] m_SortOptions = 
		{
			SortOption.StepAsset,
			SortOption.StepAsset,
			SortOption.StepAsset,
			SortOption.StepAsset,
			SortOption.StepAsset
		};

		public static void TreeToList (TreeViewItem root, IList<TreeViewItem> result)
		{
			if (root == null)
			{
				throw new NullReferenceException("root");
			}

			if (result == null)
			{
				throw new NullReferenceException("result");
			}

			result.Clear();
	
			if (root.children == null) return;

			Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
			for (int i = root.children.Count - 1; i >= 0; i--)
			{
				stack.Push(root.children[i]);
			}

			while (stack.Count > 0)
			{
				TreeViewItem current = stack.Pop();
				result.Add(current);

				if (current.hasChildren && current.children[0] != null)
				{
					for (int i = current.children.Count - 1; i >= 0; i--)
					{
						stack.Push(current.children[i]);
					}
				}
			}
		}

		public MultiColumnTreeView(TreeViewState state, MultiColumnHeader multicolumnHeader, TreeModel<StepTreeViewData> model) : base(state, multicolumnHeader, model)
		{
			Assert.AreEqual(m_SortOptions.Length, Enum.GetValues(typeof(MyColumns)).Length, "Ensure number of sort options are in sync with number of MyColumns enum values");

			// Custom setup
			rowHeight = kRowHeights;
			columnIndexForTreeFoldouts = 1;
			showBorder = true;
			customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
			extraSpaceBeforeIconAndLabel = kToggleWidth;
			multicolumnHeader.sortingChanged += OnSortingChanged;
			
			Reload();
		}


		// Note we We only build the visible rows, only the backend has the full tree information. 
		// The treeview only creates info for the row list.
		protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
		{
			var rows = base.BuildRows(root);
			SortIfNeeded(root, rows);
			return rows;
		}

		void OnSortingChanged(MultiColumnHeader multiColumnHeader)
		{
			SortIfNeeded(rootItem, GetRows());
		}

		void SortIfNeeded(TreeViewItem root, IList<TreeViewItem> rows)
		{
			if (rows.Count <= 1) return;
			
			// No column to sort for (just use the order the data are in)
			if (multiColumnHeader.sortedColumnIndex == -1) return;
			
			// Sort the roots of the existing tree items
			SortByMultipleColumns();
			TreeToList(root, rows);
			Repaint();
		}

		void SortByMultipleColumns()
		{
			var sortedColumns = multiColumnHeader.state.sortedColumns;

			if (sortedColumns.Length == 0) return;

			var myTypes = rootItem.children.Cast<TreeViewItem<StepTreeViewData>>();
			var orderedQuery = InitialOrder(myTypes, sortedColumns);
			for (int i = 1; i < sortedColumns.Length; i++)
			{
				SortOption sortOption = m_SortOptions[sortedColumns[i]];
				bool ascending = multiColumnHeader.IsSortedAscending(sortedColumns[i]);

				switch (sortOption)
				{
					case SortOption.StepAsset:
						orderedQuery = orderedQuery.ThenBy(l => l.data.name, ascending);
						break;
				}
			}

			rootItem.children = orderedQuery.Cast<TreeViewItem> ().ToList ();
		}

		IOrderedEnumerable<TreeViewItem<StepTreeViewData>> InitialOrder(IEnumerable<TreeViewItem<StepTreeViewData>> myTypes, int[] history)
		{
			SortOption sortOption = m_SortOptions[history[0]];
			bool ascending = multiColumnHeader.IsSortedAscending(history[0]);
			switch (sortOption)
			{
				case SortOption.StepAsset:
					return myTypes.Order(l => l.data.name, ascending);
				default:
					Assert.IsTrue(false, "Unhandled enum");
					break;
			}

			// default
			return myTypes.Order(l => l.data.name, ascending);
		}

		protected override void RowGUI(RowGUIArgs args)
		{
			var item = (TreeViewItem<StepTreeViewData>) args.item;

			for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
			{
				CellGUI(args.GetCellRect(i), ref item, (MyColumns)args.GetColumn(i), ref args);
			}
		}

		void CellGUI(Rect cellRect, ref TreeViewItem<StepTreeViewData> item, MyColumns column, ref RowGUIArgs args)
		{
			// Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
			CenterRectUsingSingleLineHeight(ref cellRect);

			switch (column)
			{
				case MyColumns.Icon:
				{
					if (item.data.icon == null) break;

					GUI.DrawTexture(cellRect, item.data.icon, ScaleMode.ScaleToFit);
					break;
				}
				case MyColumns.StepAsset:
				{
					Rect indentedRect = cellRect;
					var indentDisplacement = GetContentIndent(item);
					indentedRect.x += indentDisplacement;
					indentedRect.width -= indentDisplacement + 5;

					item.data.step = (BaseCommonStep)EditorGUI.ObjectField(indentedRect, GUIContent.none, item.data.step, typeof(BaseCommonStep), false);
					break;
				}
				case MyColumns.IsOptional:
				{
					cellRect.xMin += 5f;
					item.data.isOptional = EditorGUI.Toggle(cellRect, item.data.isOptional);
					break;
				}
				case MyColumns.Weight:
				{
					if (item.data.isOptional)
					{
						GUI.enabled = false;
						item.data.weight = EditorGUI.IntSlider(cellRect, GUIContent.none, 0, 0, 100);
						GUI.enabled = true;
						break;
					}
					if (item.data.weight < 1)
					{
						item.data.weight = 1;
					}
					item.data.weight = EditorGUI.IntSlider(cellRect, item.data.weight, 1, 100);
					break;
				}
				case MyColumns.Description:
				{
					item.data.description = GUI.TextField(cellRect, item.data.description);
					break;
				}
			}
		}

		protected override void SelectionChanged(IList<int> selectedIds)
		{
			if (selectedIds.Count > 0)
			{
				CurrentSelectedElement = Tree.Find(selectedIds[0]);
			}
		}

		public void NotifySelectedItemDeletion()
		{
			CurrentSelectedElement = null;
		}

		protected override bool CanMultiSelect(TreeViewItem item)
		{
			return false;
		}

		public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
		{
			var columns = new[] 
			{
				new MultiColumnHeaderState.Column 
				{
					headerContent = new GUIContent("Icon", "Maybe this step needs a little touch."),
					headerTextAlignment = TextAlignment.Center,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Center,
					width = 30, 
					minWidth = 30,
					autoResize = false,
					allowToggleVisibility = true
				},
				new MultiColumnHeaderState.Column 
				{
					headerContent = new GUIContent("Asset", "The actual step asset being used."),
					headerTextAlignment = TextAlignment.Center,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Center,
					width = 250, 
					minWidth = 200,
					autoResize = true
				},
				new MultiColumnHeaderState.Column 
				{
					headerContent = new GUIContent("Optional?", "Determines whether this step MUST be executed with success."),
					headerTextAlignment = TextAlignment.Center,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Center,
					width = 65,
					minWidth = 65,
					maxWidth = 75
				},
				new MultiColumnHeaderState.Column 
				{
					headerContent = new GUIContent("Weight", "When not optional, determines the weight of this asset when calculating progress percentage."),
					headerTextAlignment = TextAlignment.Center,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Left,
					width = 160,
					minWidth = 150,
					maxWidth = 180,
					autoResize = true,
					allowToggleVisibility = true
				},
				new MultiColumnHeaderState.Column 
				{
					headerContent = new GUIContent("Description", "Interesting things about this step."),
					headerTextAlignment = TextAlignment.Center,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Left,
					width = 250,
					minWidth = 150,
					autoResize = true
				}
			};

			Assert.AreEqual(columns.Length, Enum.GetValues(typeof(MyColumns)).Length, "Number of columns should match number of enum values: You probably forgot to update one of them.");

			var state =  new MultiColumnHeaderState(columns);
			return state;
		}
	}

	static class MyExtensionMethods
	{
		public static IOrderedEnumerable<T> Order<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector, bool ascending)
		{
			if (ascending)
			{
				return source.OrderBy(selector);
			}
			else
			{
				return source.OrderByDescending(selector);
			}
		}

		public static IOrderedEnumerable<T> ThenBy<T, TKey>(this IOrderedEnumerable<T> source, Func<T, TKey> selector, bool ascending)
		{
			if (ascending)
			{
				return source.ThenBy(selector);
			}
			else
			{
				return source.ThenByDescending(selector);
			}
		}
	}
}
