using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace StepSystem.SimpleTree.Editor {
	internal class TreeViewItem<T> : TreeViewItem where T : TreeElement {
		public T Data { get; }

		public TreeViewItem(int id, int depth, string displayName, T data) : base(id, depth, displayName) {
			Data = data;
		}
	}

	internal class TreeViewWithTreeModel<T> : TreeView where T : TreeElement {
		const string GENERIC_DRAG_ID = "GenericDragColumnDragging";

		readonly List<TreeViewItem> _rowValues = new List<TreeViewItem>(100);
		public event Action TreeChanged;

		public TreeModel<T> Tree { get; private set; }

		public event Action<IList<TreeViewItem>> BeforeDroppingDraggedItems;

		public TreeViewWithTreeModel(TreeViewState state, TreeModel<T> model) : base(state) {
			Init(model);
		}

		public TreeViewWithTreeModel(TreeViewState state, MultiColumnHeader multiColumnHeader, TreeModel<T> model)
			: base(state, multiColumnHeader) {
			Init(model);
		}

		void Init(TreeModel<T> model) {
			Tree = model;
			Tree.ModelChanged += ModelChanged;
		}

		void ModelChanged() {
			TreeChanged?.Invoke();
			Reload();
		}

		protected override TreeViewItem BuildRoot() {
			const int depthForHiddenRoot = -1;
			return new TreeViewItem<T>(Tree.Root.ID, depthForHiddenRoot, Tree.Root.Name, Tree.Root);
		}

		protected override IList<TreeViewItem> BuildRows(TreeViewItem root) {
			if (Tree.Root == null) {
				Debug.LogError("tree model root is null. did you call SetData()?");
			}

			_rowValues.Clear();
			if (!string.IsNullOrEmpty(searchString)) {
				Search(Tree.Root, searchString, _rowValues);
			}
			else if (Tree.Root != null && Tree.Root.HasChildren) {
				AddChildrenRecursive(Tree.Root, 0, _rowValues);
			}

			// We still need to setup the child parent information for the rows since this 
			// information is used by the TreeView internal logic (navigation, dragging etc)
			SetupParentsAndChildrenFromDepths(root, _rowValues);

			return _rowValues;
		}

		void AddChildrenRecursive(T parent, int depth, IList<TreeViewItem> newRows) {
			foreach (TreeElement treeElement in parent.Children) {
				var child = (T)treeElement;
				var item = new TreeViewItem<T>(child.ID, depth, child.Name, child);
				newRows.Add(item);

				if (!child.HasChildren) continue;
				
				if (IsExpanded(child.ID)) {
					AddChildrenRecursive(child, depth + 1, newRows);
				}
				else {
					item.children = CreateChildListForCollapsedParent();
				}
			}
		}

		void Search(T searchFromThis, string search, List<TreeViewItem> result) {
			if (string.IsNullOrEmpty(search)) {
				throw new ArgumentException("Invalid search: cannot be null or empty", nameof(search));
			}

			const int itemDepth = 0; // tree is flattened when searching

			var stack = new Stack<T>();
			foreach (TreeElement element in searchFromThis.Children) {
				stack.Push((T)element);
			}

			while (stack.Count > 0) {
				T current = stack.Pop();
				// Matches search?
				if (current.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0) {
					result.Add(new TreeViewItem<T>(current.ID, itemDepth, current.Name, current));
				}

				if (current.Children == null || current.Children.Count <= 0) continue;
				
				foreach (TreeElement element in current.Children) {
					stack.Push((T)element);
				}
			}

			SortSearchResult(result);
		}

		protected virtual void SortSearchResult(List<TreeViewItem> rows) {
			rows.Sort((x, y) =>
				EditorUtility.NaturalCompare(x.displayName, y.displayName)); // sort by displayName by default
		}

		protected override IList<int> GetAncestors(int id) {
			return Tree.GetAncestors(id);
		}

		protected override IList<int> GetDescendantsThatHaveChildren(int id) {
			return Tree.GetDescendantsThatHaveChildren(id);
		}

		protected override void BeforeRowsGUI() {
			if (Event.current.rawType != UnityEngine.EventType.Repaint) return;

			int count = GetRows().Count;
			if (count <= 0) return;

			GetFirstAndLastVisibleRows(out int firstRow, out int _);
			if (firstRow < 0 || firstRow >= count) return;

			float height = treeViewRect.height + state.scrollPos.y;
			var position = new Rect(0f, 0f, 100000f, rowHeight);
			int row = firstRow;
			while (position.yMax < (double)height) {
				if (row % 2 == 1) {
					if (row < count) {
						position = GetRowRect(row);
					}
					else {
						position.y += rowHeight * 2;
					}

					DefaultStyles.backgroundEven.Draw(position, false, false, false, false);
				}

				row++;
			}
		}

		protected override bool CanStartDrag(CanStartDragArgs args) {
			return true;
		}

		protected override void SetupDragAndDrop(SetupDragAndDropArgs args) {
			if (hasSearch) return;

			DragAndDrop.PrepareStartDrag();
			List<TreeViewItem> draggedRows = GetRows().Where(item => args.draggedItemIDs.Contains(item.id)).ToList();
			DragAndDrop.SetGenericData(GENERIC_DRAG_ID, draggedRows);
			DragAndDrop.objectReferences = new UnityEngine.Object[] { }; // this IS required for dragging to work
			string title = draggedRows.Count == 1 ? draggedRows[0].displayName : "< Multiple >";
			DragAndDrop.StartDrag(title);
		}

		protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args) {
			// Check if we can handle the current drag data (could be dragged in from other areas/windows in the editor)
			var draggedRows = DragAndDrop.GetGenericData(GENERIC_DRAG_ID) as List<TreeViewItem>;
			if (draggedRows == null) {
				return DragAndDropVisualMode.None;
			}

			// Parent item is null when dragging outside any tree view items.
			switch (args.dragAndDropPosition) {
				case DragAndDropPosition.UponItem:
				case DragAndDropPosition.BetweenItems: {
					bool validDrag = ValidDrag(args.parentItem, draggedRows);
					if (args.performDrop && validDrag) {
						T parentData = ((TreeViewItem<T>)args.parentItem).Data;
						OnDropDraggedElementsAtIndex(draggedRows, parentData,
							args.insertAtIndex == -1 ? 0 : args.insertAtIndex);
					}

					return validDrag ? DragAndDropVisualMode.Move : DragAndDropVisualMode.None;
				}
				case DragAndDropPosition.OutsideItems: {
					if (args.performDrop) {
						OnDropDraggedElementsAtIndex(draggedRows, Tree.Root, Tree.Root.Children.Count);
					}

					return DragAndDropVisualMode.Move;
				}
				default: {
					Debug.LogError("Unhandled enum " + args.dragAndDropPosition);
					return DragAndDropVisualMode.None;
				}
			}
		}

		public virtual void OnDropDraggedElementsAtIndex(List<TreeViewItem> draggedRows, T parent, int insertIndex) {
			BeforeDroppingDraggedItems?.Invoke(draggedRows);

			var draggedElements = new List<TreeElement>();
			foreach (TreeViewItem x in draggedRows)
				draggedElements.Add(((TreeViewItem<T>)x).Data);

			int[] selectedIDs = draggedElements.Select(x => x.ID).ToArray();
			Tree.MoveElements(parent, insertIndex, draggedElements);
			SetSelection(selectedIDs, TreeViewSelectionOptions.RevealAndFrame);
		}

		bool ValidDrag(TreeViewItem parent, List<TreeViewItem> draggedItems) {
			TreeViewItem currentParent = parent;
			while (currentParent != null) {
				if (draggedItems.Contains(currentParent))
					return false;
				currentParent = currentParent.parent;
			}

			return true;
		}
	}
}
