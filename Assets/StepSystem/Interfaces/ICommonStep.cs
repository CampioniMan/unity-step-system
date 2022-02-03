using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StepSystem.Interfaces
{
	public interface ICommonStep : IBaseStep
	{
		void Execute(Action<bool> onFinish);
	}
}