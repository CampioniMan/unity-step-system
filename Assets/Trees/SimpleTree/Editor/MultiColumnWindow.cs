using System;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditor.TreeViewExamples
{
	class MultiColumnWindow : EditorWindow
	{
		[NonSerialized] bool isInitialized;
		[SerializeField] TreeViewState treeViewState; // Serialized in the window layout file so it survives assembly reloading
		[SerializeField] MultiColumnHeaderState multiColumnHeaderState;

		SearchField searchField;
		MultiColumnTreeView treeView;
		StepTreeViewData stepTreeViewDataList;
		TreeModel<StepViewData> treeModel;

		Rect multiColumnTreeViewRect 	=> new Rect(20f, 30f, position.width - 40f, position.height - 60f);
		Rect toolbarRect 				=> new Rect(20f, 10f, position.width - 40f, 20f);
		Rect bottomToolbarRect			=> new Rect(20f, position.height - 20f, position.width - 40f, 20f);
		Rect stepTreeAssetRect 			=> new Rect(180f, position.height - 18f, Mathf.Max(140f, Mathf.Min(350f, position.width - 625f)), 16f);
		
		public MultiColumnTreeView view => treeView;

		[MenuItem("Step Tree/Open Window")]
		public static MultiColumnWindow GetWindow()
		{
			var window = GetWindow<MultiColumnWindow>();
			window.titleContent = new GUIContent("Step Tree View");
			window.Focus();
			window.Repaint();
			return window;
		}

		void SetTreeAsset(StepTreeViewData myTreeAsset)
		{
			stepTreeViewDataList = myTreeAsset;
			isInitialized = false;
		}

		void InitIfNeeded()
		{
			if (!isInitialized)
			{
				// Check if it already exists (deserialized from window layout file or scriptable object)
				if (treeViewState == null)
				{
					treeViewState = new TreeViewState();
				}

				bool firstInit = multiColumnHeaderState == null;
				var headerState = MultiColumnTreeView.CreateDefaultMultiColumnHeaderState(multiColumnTreeViewRect.width);
				if (MultiColumnHeaderState.CanOverwriteSerializedFields(multiColumnHeaderState, headerState))
				{
					MultiColumnHeaderState.OverwriteSerializedFields(multiColumnHeaderState, headerState);
				}
				multiColumnHeaderState = headerState;
				
				var multiColumnHeader = new MyMultiColumnHeader(headerState);
				if (firstInit)
				{
					multiColumnHeader.ResizeToFit();
				}

				treeModel = new TreeModel<StepViewData>(stepTreeViewDataList.treeElements);
				
				treeView = new MultiColumnTreeView(treeViewState, multiColumnHeader, treeModel);

				searchField = new SearchField();
				searchField.downOrUpArrowKeyPressed += treeView.SetFocusAndEnsureSelectedItem;

				isInitialized = true;
			}
		}

		bool IsListDefined()
		{
			return stepTreeViewDataList != null && stepTreeViewDataList.treeElements != null && stepTreeViewDataList.treeElements.Count > 0;
		}

		void OnGUI()
		{
			BottomToolBar(bottomToolbarRect);
			if (!IsListDefined()) return;

			InitIfNeeded();
			SearchBar(toolbarRect);
			DoTreeView(multiColumnTreeViewRect);
		}

		void SearchBar(Rect rect)
		{
			view.searchString = searchField.OnGUI(rect, view.searchString);
		}

		void DoTreeView(Rect rect)
		{
			treeView.OnGUI(rect);
		}

		void BottomToolBar(Rect rect)
		{
			GUILayout.BeginArea(rect);

			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.Label("Step List: ");
				stepTreeViewDataList = (StepTreeViewData)EditorGUILayout.ObjectField( stepTreeViewDataList, typeof(StepTreeViewData), false, GUILayout.MinWidth(200), GUILayout.MaxWidth(5000));
				if (!IsListDefined())
				{
					GUILayout.EndArea();
					return;
				}

				GUILayout.FlexibleSpace();

				var style = "miniButton";
				if (view?.CurrentSelectedElement == null)
				{
					GUI.enabled = false;
				}

				if (GUILayout.Button("+", style))
				{
					treeModel.AddElement(new StepViewData(view.CurrentSelectedElement.depth + 1, treeModel.GenerateUniqueID(), null, "", false),
						view.CurrentSelectedElement, view.CurrentSelectedElement.children?.Count ?? 0);

				}

				if (GUILayout.Button("-", style))
				{
					//TODO: Delete multiple rows
					treeModel.RemoveElements(new List<StepViewData>() { view.CurrentSelectedElement });
					view.NotifySelectedItemDeletion();
				}

				GUI.enabled = true;

				if (GUILayout.Button("Expand All", style))
				{
					view.ExpandAll();
				}

				if (GUILayout.Button("Collapse All", style))
				{
					view.CollapseAll();
				}
			}

			GUILayout.EndArea();
		}
	}


	internal class MyMultiColumnHeader : MultiColumnHeader
	{
		Mode m_Mode;

		public enum Mode
		{
			LargeHeader,
			DefaultHeader,
			MinimumHeaderWithoutSorting
		}

		public MyMultiColumnHeader(MultiColumnHeaderState state)
			: base(state)
		{
			mode = Mode.DefaultHeader;
		}

		public Mode mode
		{
			get
			{
				return m_Mode;
			}
			set
			{
				m_Mode = value;
				switch (m_Mode)
				{
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
