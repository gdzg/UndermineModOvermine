using BehaviorDesigner.Runtime.Tasks;
using Thor;
using UnityEngine;

[TaskCategory("Mod")]
public class CompareEntityGuid : Action
{
    [SerializeField] [RequiredField] private string guid;
    [SerializeField] [RequiredField] private SharedEntity input;

    public override TaskStatus OnUpdate()
    {
        return input.Value.Guid.Equals(guid) ? TaskStatus.Success : TaskStatus.Failure;
    }
}