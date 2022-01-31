using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EditableTreeNode
{
    public ScriptableObject Value => _value;
    [SerializeField] public ScriptableObject _value;

    public List<EditableTreeNode> Children => _children;
    [SerializeField] public List<EditableTreeNode> _children;

    [field: SerializeField] public int Height { get; }

    public EditableTreeNode(ScriptableObject newValue, int newHeight)
    {
        _value = newValue;
        Height = newHeight;
        _children = new List<EditableTreeNode>();
    }

    public EditableTreeNode(EditableTreeNode original)
    {
        _value = original.Value;
        Height = original.Height;
        _children = new List<EditableTreeNode>(original.Children);
    }

    public void InsertChild(ScriptableObject value)
    {
        Children.Add(new EditableTreeNode(value, Height + 1));
    }

    public bool DeleteChild(ScriptableObject value)
    {
        for (int i = Children.Count - 1; i >= 0; i--)
        {
            EditableTreeNode child = Children[i];
            if (child.Value.Equals(value))
            {
                Children.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public EditableTreeNode Clone()
    {
        return new EditableTreeNode(this);
    }
}
