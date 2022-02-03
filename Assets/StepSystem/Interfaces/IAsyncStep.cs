using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace StepSystem.Interfaces
{
	public interface IAsyncStep : IBaseStep
	{
		Task<bool> ExecuteTask();
	}
}