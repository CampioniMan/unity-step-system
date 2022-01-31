using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

class AnimalTreeWindow : EditorWindow
{
	// We are using SerializeField here to make sure view state is written to the window 
	// layout file. This means that the state survives restarting Unity as long as the window
	// is not closed. If omitting the attribute then the state just survives assembly reloading 
	// (i.e. it still gets serialized/deserialized)
	[SerializeField] TreeViewState m_TreeViewState;

	// The TreeView is not serializable it should be reconstructed from the tree data.
	AnimalTree m_SimpleTreeView;

	void OnEnable()
	{
		// Check if we already had a serialized view state (state 
		// that survived assembly reloading)
		if (m_TreeViewState == null)
			m_TreeViewState = new TreeViewState();

		m_SimpleTreeView = new AnimalTree(m_TreeViewState);
	}

	void OnGUI()
	{
		m_SimpleTreeView.OnGUI(new Rect(0, 0, position.width, position.height));
	}

	// Add menu named "My Window" to the Window menu
	[MenuItem("TreeView Examples/Animal Tree Window")]
	static void ShowWindow()
	{
		// Get existing open window or if none, make a new one:
		var window = GetWindow<AnimalTreeWindow>();
		window.titleContent = new GUIContent("My Window");
		window.Show();
	}
}
