using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StepSystem.Interfaces
{
	//[CreateAssetMenu(fileName = "New Common Step",
	//	menuName = "Step System/Common Step", order = 1)]
	public interface ICommonStep : IBaseStep
	{
		void Prepare();
		void Execute();
		void Dispose();
	}
}