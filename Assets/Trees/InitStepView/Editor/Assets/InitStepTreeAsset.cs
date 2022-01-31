using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

[CreateAssetMenu(fileName = "InitStepTreeAssetDataAsset", menuName = "Init Step Tree Asset", order = 1)]
public class InitStepTreeAsset : ScriptableObject
{
    [SerializeField] List<InitStepTreeElement> treeElements = new List<InitStepTreeElement>();

    internal List<InitStepTreeElement> TreeElements
    {
        get { return treeElements; }
        set { treeElements = value; }
    }
}

