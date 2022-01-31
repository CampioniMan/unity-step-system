using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestSO", menuName = "ScriptableObjects/TestScriptableObject", order = 1)]
public class TestScriptableObject : ScriptableObject
{
    [SerializeField] int _thing;
}
