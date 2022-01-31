using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(SOEditableTree))]
public class SOEditableTreeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();
        base.OnInspectorGUI();
        if (GUILayout.Button("Randomize Sine Function", EditorStyles.miniButton))
        {

        }
    }
}
