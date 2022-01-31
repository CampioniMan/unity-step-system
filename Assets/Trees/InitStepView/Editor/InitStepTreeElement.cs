using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

[Serializable]
internal class InitStepTreeElement : TreeViewItem
{
    public bool isBlocker;
    public bool isCoroutine;

    public InitStepTreeElement(string name, int depth, int id)
        : base(depth, id, name)
    {
        isBlocker = false;
        isCoroutine = false;
    }
}

