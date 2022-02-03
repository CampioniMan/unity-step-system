using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace StepSystem.Interfaces
{
	//[CreateAssetMenu(fileName = "New Async Step",
	//	menuName = "Step System/Async Step", order = 1)]
	public interface IAsyncStep : IBaseStep
	{
		void Prepare();
		Task Execute();
		void Dispose();
	}
}