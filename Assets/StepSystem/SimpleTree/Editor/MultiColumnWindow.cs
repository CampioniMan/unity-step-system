using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace StepSystem.SimpleTree.Editor {
	class MultiColumnWindow : EditorWindow {
		[NonSerialized] bool _isInitialized;

		[SerializeField]
		TreeViewState treeViewState; // Serialized in the window layout file so it survives assembly reloading

		[SerializeField] MultiColumnHeaderState multiColumnHeaderState;

		SearchField _searchField;
		StepTreeViewData _stepTreeViewDataList;
		TreeModel<StepViewData> _treeModel;

		Rect MultiColumnTreeViewRect => new Rect(20f, 30f, position.width - 40f, position.height - 60f);
		Rect ToolbarRect => new Rect(20f, 10f, position.width - 40f, 20f);
		Rect BottomToolbarRect => new Rect(20f, position.height - 20f, position.width - 40f, 20f);
		Rect StepTreeAssetRect => new Rect(180f, position.height - 18f,
			Mathf.Max(140f, Mathf.Min(350f, position.width - 625f)), 16f);
		public MultiColumnTreeView View { get; private set; }

		[MenuItem("Step Tree/Open Window")]
		public static void GetWindow() {
			var window = GetWindow<MultiColumnWindow>();
			window.titleContent = new GUIContent("Step Tree View");
			window.Focus();
			window.Repaint();
		}

		void SetTreeAsset(StepTreeViewData myTreeAsset) {
			_stepTreeViewDataList = myTreeAsset;
			_isInitialized = false;
		}

		void InitIfNeeded() {
			if (_isInitialized) return;
			
			// Check if it already exists (deserialized from window layout file or scriptable object)
			treeViewState ??= new TreeViewState();

			bool firstInit = multiColumnHeaderState == null;
			MultiColumnHeaderState headerState =
				MultiColumnTreeView.CreateDefaultMultiColumnHeaderState(MultiColumnTreeViewRect.width);
			if (MultiColumnHeaderState.CanOverwriteSerializedFields(multiColumnHeaderState, headerState)) {
				MultiColumnHeaderState.OverwriteSerializedFields(multiColumnHeaderState, headerState);
			}

			multiColumnHeaderState = headerState;

			var multiColumnHeader = new MyMultiColumnHeader(headerState);
			if (firstInit) {
				multiColumnHeader.ResizeToFit();
			}

			_treeModel = new TreeModel<StepViewData>(_stepTreeViewDataList.TreeElements);

			View = new MultiColumnTreeView(treeViewState, multiColumnHeader, _treeModel);

			_searchField = new SearchField();
			_searchField.downOrUpArrowKeyPressed += View.SetFocusAndEnsureSelectedItem;

			_isInitialized = true;
		}

		bool IsListDefined() {
			return _stepTreeViewDataList != null && _stepTreeViewDataList.TreeElements != null &&
			       _stepTreeViewDataList.TreeElements.Count > 0;
		}

		void OnGUI() {
			BottomToolBar(BottomToolbarRect);
			if (!IsListDefined()) return;

			InitIfNeeded();
			SearchBar(ToolbarRect);
			DoTreeView(MultiColumnTreeViewRect);
		}

		void SearchBar(Rect rect) {
			View.searchString = _searchField.OnGUI(rect, View.searchString);
		}

		void DoTreeView(Rect rect) {
			View.OnGUI(rect);
		}

		void BottomToolBar(Rect rect) {
			GUILayout.BeginArea(rect);

			using (new EditorGUILayout.HorizontalScope()) {
				GUILayout.Label("Step Tree: ");
				_stepTreeViewDataList = (StepTreeViewData)EditorGUILayout.ObjectField(_stepTreeViewDataList,
					typeof(StepTreeViewData), false, GUILayout.MinWidth(200), GUILayout.MaxWidth(5000));
				if (!IsListDefined()) {
					GUILayout.EndArea();
					return;
				}

				GUILayout.FlexibleSpace();

				const string style = "miniButton";
				if (View?.CurrentSelectedElement == null) {
					GUI.enabled = false;
				}

				if (GUILayout.Button("+", style)) {
					if (View?.CurrentSelectedElement != null) {
						_treeModel.AddElement(
							new StepViewData(View.CurrentSelectedElement.Depth + 1, _treeModel.GenerateUniqueID(),
								null,
								"",
								false),
							View.CurrentSelectedElement, View.CurrentSelectedElement.Children?.Count ?? 0);
					}
				}

				if (GUILayout.Button("-", style)) {
					//TODO: Delete multiple rows
					_treeModel.RemoveElements(new List<StepViewData>() { View.CurrentSelectedElement });
					View.NotifySelectedItemDeletion();
				}

				GUI.enabled = true;

				if (GUILayout.Button("Expand All", style)) {
					View?.ExpandAll();
				}

				if (GUILayout.Button("Collapse All", style)) {
					View?.CollapseAll();
				}
			}

			GUILayout.EndArea();
		}
	}


	internal class MyMultiColumnHeader : MultiColumnHeader {
		Mode _currentMode;

		public enum Mode {
			LargeHeader,
			DefaultHeader,
			MinimumHeaderWithoutSorting
		}

		public MyMultiColumnHeader(MultiColumnHeaderState state)
			: base(state) {
			CurrentMode = Mode.DefaultHeader;
		}

		public Mode CurrentMode {
			get => _currentMode;
			set {
				_currentMode = value;
				switch (_currentMode) {
					case Mode.LargeHeader:
						canSort = true;
						height = 37f;
						break;
					case Mode.DefaultHeader:
						canSort = true;
						height = DefaultGUI.defaultHeight;
						break;
					case Mode.MinimumHeaderWithoutSorting:
						canSort = false;
						height = DefaultGUI.minimumHeight;
						break;
				}
			}
		}
	}
}
