using BehaviorDesigner.Runtime.Tasks;
using Thor;

[TaskCategory("Mod")]
	public class HasVisitHalfRoom : Conditional
	{
		// Token: 0x06002792 RID: 10130
		public override TaskStatus OnUpdate()
		{
			var rooms = Game.Instance.Simulation.Zone.Rooms;
			if (rooms.FindAll(s => s.Visited).Count > rooms.Count / 2)
			{
				return TaskStatus.Failure;
			}
			return TaskStatus.Success;
		}
	}

