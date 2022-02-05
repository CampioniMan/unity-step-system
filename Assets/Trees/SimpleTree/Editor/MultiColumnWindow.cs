using System;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditor.TreeViewExamples
{
	class MultiColumnWindow : EditorWindow
	{
		[NonSerialized] bool m_Initialized;
		[SerializeField] TreeViewState m_TreeViewState; // Serialized in the window layout file so it survives assembly reloading
		[SerializeField] MultiColumnHeaderState m_MultiColumnHeaderState;
		SearchField m_SearchField;
		MultiColumnTreeView m_TreeView;
		StepTreeViewDataList m_StepTreeViewDataList;
		TreeModel<StepTreeViewData> treeModel;

		Rect multiColumnTreeViewRect => new Rect(20f, 30f, position.width - 40f, position.height - 60f);
		Rect toolbarRect => new Rect(20f, 10f, position.width - 40f, 20f);
		Rect bottomToolbarRect => new Rect(20f, position.height - 20f, position.width - 40f, 20f);
		Rect stepTreeAssetRect => new Rect(180f, position.height - 18f, Mathf.Max(140f, Mathf.Min(350f, position.width - 625f)), 16f);
		
		public MultiColumnTreeView treeView => m_TreeView;

		[MenuItem("Step Tree/Open Window")]
		public static MultiColumnWindow GetWindow()
		{
			var window = GetWindow<MultiColumnWindow>();
			window.titleContent = new GUIContent("Step Tree View");
			window.Focus();
			window.Repaint();
			return window;
		}

		void SetTreeAsset(StepTreeViewDataList myTreeAsset)
		{
			m_StepTreeViewDataList = myTreeAsset;
			m_Initialized = false;
		}

		void InitIfNeeded()
		{
			if (!m_Initialized)
			{
				// Check if it already exists (deserialized from window layout file or scriptable object)
				if (m_TreeViewState == null)
				{
					m_TreeViewState = new TreeViewState();
				}

				bool firstInit = m_MultiColumnHeaderState == null;
				var headerState = MultiColumnTreeView.CreateDefaultMultiColumnHeaderState(multiColumnTreeViewRect.width);
				if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderState, headerState))
				{
					MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderState, headerState);
				}
				m_MultiColumnHeaderState = headerState;
				
				var multiColumnHeader = new MyMultiColumnHeader(headerState);
				if (firstInit)
				{
					multiColumnHeader.ResizeToFit();
				}

				treeModel = new TreeModel<StepTreeViewData>(m_StepTreeViewDataList.treeElements);
				
				m_TreeView = new MultiColumnTreeView(m_TreeViewState, multiColumnHeader, treeModel);

				m_SearchField = new SearchField();
				m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;

				m_Initialized = true;
			}
		}

		bool IsListDefined()
		{
			return m_StepTreeViewDataList != null && m_StepTreeViewDataList.treeElements != null && m_StepTreeViewDataList.treeElements.Count > 0;
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
			treeView.searchString = m_SearchField.OnGUI(rect, treeView.searchString);
		}

		void DoTreeView(Rect rect)
		{
			m_TreeView.OnGUI(rect);
		}

		void BottomToolBar(Rect rect)
		{
			GUILayout.BeginArea(rect);

			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.Label("Step List: ");
				m_StepTreeViewDataList = (StepTreeViewDataList)EditorGUILayout.ObjectField( m_StepTreeViewDataList, typeof(StepTreeViewDataList), false, GUILayout.MinWidth(200), GUILayout.MaxWidth(5000));
				if (!IsListDefined())
				{
					GUILayout.EndArea();
					return;
				}

				GUILayout.FlexibleSpace();

				var style = "miniButton";
				if (treeView?.CurrentSelectedElement == null)
				{
					GUI.enabled = false;
				}

				if (GUILayout.Button("+", style))
				{
					treeModel.AddElement(new StepTreeViewData(treeView.CurrentSelectedElement.depth + 1, treeModel.GenerateUniqueID(), null, "", false),
						treeView.CurrentSelectedElement, treeView.CurrentSelectedElement.children?.Count ?? 0);

				}

				if (GUILayout.Button("-", style))
				{
					//TODO: Delete multiple rows
					treeModel.RemoveElements(new List<StepTreeViewData>() { treeView.CurrentSelectedElement });
					treeView.NotifySelectedItemDeletion();
				}

				GUI.enabled = true;

				if (GUILayout.Button("Expand All", style))
				{
					treeView.ExpandAll();
				}

				if (GUILayout.Button("Collapse All", style))
				{
					treeView.CollapseAll();
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
