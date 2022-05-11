using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace StepSystem.SimpleTree
{
	[Serializable]
	public class TreeElement
	{
		[FormerlySerializedAs("mID")] [FormerlySerializedAs("m_ID")] [SerializeField] int elementId;
		[FormerlySerializedAs("mName")] [FormerlySerializedAs("m_Name")] [SerializeField] string elementName;
		[FormerlySerializedAs("mDepth")] [FormerlySerializedAs("m_Depth")] [SerializeField] int elementDepth;
		[NonSerialized] TreeElement _elementParent;
		[NonSerialized] List<TreeElement> _elementChildren;

		public int Depth
		{
			get => elementDepth;
			set => elementDepth = value;
		}

		public TreeElement Parent
		{
			get => _elementParent;
			set => _elementParent = value;
		}

		public List<TreeElement> Children
		{
			get => _elementChildren;
			set => _elementChildren = value;
		}

		public bool HasChildren => Children != null && Children.Count > 0;

		public string Name
		{
			get => elementName;
			set => elementName = value;
		}

		public int ID
		{
			get => elementId;
			set => elementId = value;
		}

		public TreeElement()
		{
		}

		public TreeElement(string name, int depth, int id)
		{
			elementName = name;
			elementId = id;
			elementDepth = depth;
		}
	}
}
