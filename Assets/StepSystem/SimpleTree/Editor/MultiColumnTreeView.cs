using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace StepSystem.SimpleTree.Editor {
	internal class MultiColumnTreeView : TreeViewWithTreeModel<StepViewData> {
		const float ROW_HEIGHTS = 20f;
		const float TOGGLE_WIDTH = 18f;

		public StepViewData CurrentSelectedElement { get; private set; }

		// All columns
		enum MyColumns {
			Icon,
			StepAsset,
			IsOptional,
			Description
		}

		public enum SortOption {
			StepAsset
		}

		// Sort options per column
		readonly SortOption[] _sortOptions = {
			SortOption.StepAsset,
			SortOption.StepAsset,
			SortOption.StepAsset,
			SortOption.StepAsset
		};

		public static void TreeToList(TreeViewItem root, IList<TreeViewItem> result) {
			if (root == null) {
				throw new NullReferenceException("root");
			}

			if (result == null) {
				throw new NullReferenceException("result");
			}

			result.Clear();

			if (root.children == null) return;

			var stack = new Stack<TreeViewItem>();
			for (int i = root.children.Count - 1; i >= 0; i--) {
				stack.Push(root.children[i]);
			}

			while (stack.Count > 0) {
				TreeViewItem current = stack.Pop();
				result.Add(current);

				if (current.hasChildren && current.children[0] != null) {
					for (int i = current.children.Count - 1; i >= 0; i--) {
						stack.Push(current.children[i]);
					}
				}
			}
		}

		public MultiColumnTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader,
			TreeModel<StepViewData> model) : base(state, multiColumnHeader, model) {
			Assert.AreEqual(_sortOptions.Length, Enum.GetValues(typeof(MyColumns)).Length,
				"Ensure number of sort options are in sync with number of MyColumns enum values");

			// Custom setup
			rowHeight = ROW_HEIGHTS;
			columnIndexForTreeFoldouts = 1;
			showBorder = true;
			customFoldoutYOffset =
				(ROW_HEIGHTS - EditorGUIUtility.singleLineHeight) *
				0.5f; // center foldout in the row since we also center content. See RowGUI
			extraSpaceBeforeIconAndLabel = TOGGLE_WIDTH;
			multiColumnHeader.sortingChanged += OnSortingChanged;

			Reload();
		}


		// Note we We only build the visible rows, only the backend has the full tree information. 
		// The treeview only creates info for the row list.
		protected override IList<TreeViewItem> BuildRows(TreeViewItem root) {
			IList<TreeViewItem> rows = base.BuildRows(root);
			SortIfNeeded(root, rows);
			return rows;
		}

		void OnSortingChanged(MultiColumnHeader currentMultiColumnHeader) {
			SortIfNeeded(rootItem, GetRows());
		}

		void SortIfNeeded(TreeViewItem root, IList<TreeViewItem> rows) {
			if (rows.Count <= 1) return;

			// No column to sort for (just use the order the data are in)
			if (multiColumnHeader.sortedColumnIndex == -1) return;

			// Sort the roots of the existing tree items
			SortByMultipleColumns();
			TreeToList(root, rows);
			Repaint();
		}

		void SortByMultipleColumns() {
			int[] sortedColumns = multiColumnHeader.state.sortedColumns;

			if (sortedColumns.Length == 0) return;

			IEnumerable<TreeViewItem<StepViewData>> myTypes = rootItem.children.Cast<TreeViewItem<StepViewData>>();
			IOrderedEnumerable<TreeViewItem<StepViewData>> orderedQuery = InitialOrder(myTypes, sortedColumns);
			for (var i = 1; i < sortedColumns.Length; i++) {
				SortOption sortOption = _sortOptions[sortedColumns[i]];
				bool ascending = multiColumnHeader.IsSortedAscending(sortedColumns[i]);

				orderedQuery = sortOption switch {
					SortOption.StepAsset => orderedQuery.ThenBy(l => l.Data.Name, ascending),
					_ => orderedQuery
				};
			}

			rootItem.children = orderedQuery.Cast<TreeViewItem>().ToList();
		}

		IOrderedEnumerable<TreeViewItem<StepViewData>> InitialOrder(IEnumerable<TreeViewItem<StepViewData>> myTypes,
			IReadOnlyList<int> history) {
			SortOption sortOption = _sortOptions[history[0]];
			bool ascending = multiColumnHeader.IsSortedAscending(history[0]);
			switch (sortOption) {
				case SortOption.StepAsset:
					return myTypes.Order(l => l.Data.Name, ascending);
				default:
					Assert.IsTrue(false, "Unhandled enum");
					break;
			}

			// default
			return myTypes.Order(l => l.Data.Name, ascending);
		}

		protected override void RowGUI(RowGUIArgs args) {
			var item = (TreeViewItem<StepViewData>)args.item;

			for (var i = 0; i < args.GetNumVisibleColumns(); ++i) {
				CellGUI(args.GetCellRect(i), ref item, (MyColumns)args.GetColumn(i), ref args);
			}
		}

		void CellGUI(Rect cellRect, ref TreeViewItem<StepViewData> item, MyColumns column, ref RowGUIArgs args) {
			// Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
			CenterRectUsingSingleLineHeight(ref cellRect);

			switch (column) {
				case MyColumns.Icon: {
					if (item.Data.icon == null) break;

					GUI.DrawTexture(cellRect, item.Data.icon, ScaleMode.ScaleToFit);
					break;
				}
				case MyColumns.StepAsset: {
					Rect indentedRect = cellRect;
					float indentDisplacement = GetContentIndent(item);
					indentedRect.x += indentDisplacement;
					indentedRect.width -= indentDisplacement + 5;

					item.Data.step = (BaseCommonStep)EditorGUI.ObjectField(indentedRect, GUIContent.none,
						item.Data.step, typeof(BaseCommonStep), false);
					item.Data.Name = item.Data.step != null ? item.Data.step.name : "";

					break;
				}
				case MyColumns.IsOptional: {
					cellRect.xMin += 5f;
					item.Data.isOptional = EditorGUI.Toggle(cellRect, item.Data.isOptional);
					break;
				}
				case MyColumns.Description: {
					item.Data.description = GUI.TextField(cellRect, item.Data.description);
					break;
				}
			}
		}

		protected override void SelectionChanged(IList<int> selectedIds) {
			if (selectedIds.Count > 0) {
				CurrentSelectedElement = Tree.Find(selectedIds[0]);
			}
		}

		public void NotifySelectedItemDeletion() {
			CurrentSelectedElement = null;
		}

		protected override bool CanMultiSelect(TreeViewItem item) {
			return false;
		}

		public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth) {
			var columns = new[] {
				new MultiColumnHeaderState.Column {
					headerContent = new GUIContent("Icon", "Maybe this step needs a little touch."),
					headerTextAlignment = TextAlignment.Center,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Center,
					width = 30,
					minWidth = 30,
					autoResize = false,
					allowToggleVisibility = true
				},
				new MultiColumnHeaderState.Column {
					headerContent = new GUIContent("Asset", "The actual step asset being used."),
					headerTextAlignment = TextAlignment.Center,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Center,
					width = 250,
					minWidth = 200,
					autoResize = true
				},
				new MultiColumnHeaderState.Column {
					headerContent = new GUIContent("Optional?",
						"Determines whether this step MUST be executed with success."),
					headerTextAlignment = TextAlignment.Center,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Center,
					width = 65,
					minWidth = 65,
					maxWidth = 75
				},
				new MultiColumnHeaderState.Column {
					headerContent = new GUIContent("Description", "Interesting things about this step."),
					headerTextAlignment = TextAlignment.Center,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Left,
					width = 350,
					minWidth = 250,
					autoResize = true
				}
			};

			Assert.AreEqual(columns.Length, Enum.GetValues(typeof(MyColumns)).Length,
				"Number of columns should match number of enum values: You probably forgot to update one of them.");

			var state = new MultiColumnHeaderState(columns);
			return state;
		}
	}

	static class MyExtensionMethods {
		public static IOrderedEnumerable<T> Order<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector,
			bool ascending) {
			return ascending ? source.OrderBy(selector) : source.OrderByDescending(selector);
		}

		public static IOrderedEnumerable<T> ThenBy<T, TKey>(this IOrderedEnumerable<T> source, Func<T, TKey> selector,
			bool ascending) {
			return ascending ? source.ThenBy(selector) : source.ThenByDescending(selector);
		}
	}
}
