using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StepSystem.Interfaces;

namespace StepSystem
{
	public abstract class BaseCommonStep : ICommonStep
	{
		public abstract void Prepare();
		public abstract void Execute();
		public abstract void Dispose();
	}
}