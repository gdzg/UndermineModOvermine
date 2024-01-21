using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Thor;

[TaskCategory("Mod")]
public class IsBossZone : Action
{
    public override TaskStatus OnUpdate()
    {
        var zoneData = Game.Instance.Simulation.Zone.Data;
        if (zoneData.FloorNumber <= 0) return TaskStatus.Failure;
        if (zoneData.Mode == Game.GameMode.Rogue)
        {
            return ExampleMod.GetFieldValue<bool, ZoneData>("m_isBossZone", zoneData)
                ? TaskStatus.Success
                : TaskStatus.Failure;
        }
        return (zoneData.FloorNumber - zoneData.Index + 1) % 4 == 0 ? TaskStatus.Success : TaskStatus.Failure;

    }
}