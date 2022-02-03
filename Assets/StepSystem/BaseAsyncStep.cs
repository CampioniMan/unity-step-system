using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using StepSystem.Interfaces;

namespace StepSystem
{
	public abstract class BaseAsyncStep : IAsyncStep
	{
		public abstract void Prepare();
		public abstract Task Execute();
		public abstract void Dispose();
	}
}
