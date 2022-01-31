using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SOEditableTree
{
    public int Count => _nodeCount;

    [SerializeField] EditableTreeNode _root;
    [SerializeField] int _nodeCount;

    public void Insert(ScriptableObject value)
    {
        if (_root == null)
        {
            _root = new EditableTreeNode(value, 0);
        }
        else
        {
            _root.InsertChild(value);
        }
        _nodeCount++;
    }

    public bool InsertAfter(ScriptableObject value, ScriptableObject parent)
    {
        if (_root == null)
        {
            Insert(value);
            return true;
        }

        return InsertAfterRecursive(value, parent, _root);
    }

    bool InsertAfterRecursive(ScriptableObject value, ScriptableObject parent, EditableTreeNode currentNode)
    {
        if (currentNode.Value.Equals(parent))
        {
            currentNode.InsertChild(value);
            _nodeCount++;
            return true;
        }

        // Depth insertion
        foreach (var child in currentNode.Children)
        {
            if (InsertAfterRecursive(value, parent, child))
            {
                return true;
            }
        }
        return false;
    }

    public bool Delete(ScriptableObject value)
    {
        if (_root == null)
        {
            return false;
        }

        if (_root.Value.Equals(value))
        {
            _root = null;
        }

        return DeleteRecursive(value, _root);
    }

    bool DeleteRecursive(ScriptableObject value, EditableTreeNode currentNode)
    {
        if (currentNode.DeleteChild(value))
        {
            _nodeCount--;
            return true;
        }

        // Depth deletion
        foreach (var child in currentNode.Children)
        {
            if (DeleteRecursive(value, child))
            {
                return true;
            }
        }
        return false;
    }

    public bool HasValue(ScriptableObject value)
    {
        if (_root == null)
        {
            return false;
        }

        return HasValueRecursive(value, _root);
    }

    bool HasValueRecursive(ScriptableObject value, EditableTreeNode currentNode)
    {
        if (currentNode.Value.Equals(value))
        {
            return true;
        }

        // Depth search
        foreach (var child in currentNode.Children)
        {
            if (HasValueRecursive(value, child))
            {
                return true;
            }
        }
        return false;
    }

    public List<EditableTreeNode> ToList()
    {
        var depthList = new List<EditableTreeNode>();

        if (_root != null)
        {
            ToListRecursive(_root, ref depthList);
        }
        return depthList;
    }

    void ToListRecursive(EditableTreeNode currentNode, ref List<EditableTreeNode> depthList)
    {
        depthList.Add(currentNode.Clone());

        // Depth search
        foreach (var child in currentNode.Children)
        {
            ToListRecursive(child, ref depthList);
        }
    }
}
