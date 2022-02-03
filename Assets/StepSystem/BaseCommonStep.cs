using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StepSystem.Interfaces;

namespace StepSystem
{
	public abstract class BaseCommonStep : ScriptableObject
	{
		public abstract void Prepare();
		public abstract void Execute(Action<bool> onFinish);
		public abstract void Dispose();
	}
}