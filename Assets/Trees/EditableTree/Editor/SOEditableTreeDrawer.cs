using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SOEditableTree))]
public class SOEditableTreeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        GUI.enabled = false;
        EditorGUI.indentLevel = 0;

        var countRect = new Rect(position.x, position.y, position.width, 18);
        EditorGUI.PropertyField(countRect, property.FindPropertyRelative("_nodeCount"), new GUIContent("Total Nodes:"));

        var rootRect = new Rect(position.x, position.y + 20, position.width, 18);
        PrintNodeValue(property.FindPropertyRelative("_root"), rootRect, 0);

        var childrenProp = property.FindPropertyRelative("_root").FindPropertyRelative("_children");
        var childrenArray = new SerializedProperty[childrenProp.arraySize];
        for (int i = 0; i < childrenArray.Length; i++)
        {
            childrenArray[i] = childrenProp.GetArrayElementAtIndex(i);
            var childRect = new Rect(position.x, position.y + 20 * (i + 2), position.width, 18);
            PrintNodeValue(childrenArray[i], childRect, 1);
        }

        EditorGUI.indentLevel = 0;
        GUI.enabled = true;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 500;
    }

    void PrintNodeValue(SerializedProperty node, Rect position, int indentLevel)
    {
        EditorGUI.indentLevel = indentLevel;
        if (node != null)
        {
            EditorGUI.PropertyField(position, node.FindPropertyRelative("_value"), GUIContent.none);
        }
    }
}

public static class EditorHelper
{

    public static object GetPropertyValue(this SerializedProperty prop)
    {
        if (prop == null) throw new System.ArgumentNullException("prop");

        switch (prop.propertyType)
        {
            case SerializedPropertyType.Integer:
                return prop.intValue;
            case SerializedPropertyType.Boolean:
                return prop.boolValue;
            case SerializedPropertyType.Float:
                return prop.floatValue;
            case SerializedPropertyType.String:
                return prop.stringValue;
            case SerializedPropertyType.Color:
                return prop.colorValue;
            case SerializedPropertyType.ObjectReference:
                return prop.objectReferenceValue;
            case SerializedPropertyType.LayerMask:
                return (LayerMask)prop.intValue;
            case SerializedPropertyType.Enum:
                return prop.enumValueIndex;
            case SerializedPropertyType.Vector2:
                return prop.vector2Value;
            case SerializedPropertyType.Vector3:
                return prop.vector3Value;
            case SerializedPropertyType.Vector4:
                return prop.vector4Value;
            case SerializedPropertyType.Rect:
                return prop.rectValue;
            case SerializedPropertyType.ArraySize:
                return prop.arraySize;
            case SerializedPropertyType.Character:
                return (char)prop.intValue;
            case SerializedPropertyType.AnimationCurve:
                return prop.animationCurveValue;
            case SerializedPropertyType.Bounds:
                return prop.boundsValue;
            case SerializedPropertyType.Gradient:
                throw new System.InvalidOperationException("Can not handle Gradient types.");
        }

        return null;
    }

    public static T GetPropertyValue<T>(this SerializedProperty prop)
    {
        var obj = GetPropertyValue(prop);
        if (obj is T) return (T)obj;

        var tp = typeof(T);
        try
        {
            return (T)System.Convert.ChangeType(obj, tp);
        }
        catch (System.Exception)
        {
            return default(T);
        }
    }

    public static T[] GetAsArray<T>(this SerializedProperty prop)
    {
        if (prop == null) throw new System.ArgumentNullException("prop");
        if (!prop.isArray) throw new System.ArgumentException("SerializedProperty does not represent an Array.", "prop");

        var arr = new T[prop.arraySize];
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = GetPropertyValue<T>(prop.GetArrayElementAtIndex(i));
        }
        return arr;
    }
}
