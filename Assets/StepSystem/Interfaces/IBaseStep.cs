using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StepSystem.Interfaces
{
	public interface IBaseStep
	{
		void Prepare();
		void Dispose();
	}
}
