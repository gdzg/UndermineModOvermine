using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Thor;

[TaskCategory("Mod")]
public class ResetSeed : Action
{
	public override TaskStatus OnUpdate()
	{
		Game.Instance.Simulation.MasterSeed = Rand.NewSeed;
		return TaskStatus.Success;
	}
}