using BehaviorDesigner.Runtime.Tasks;
using Thor;
using UnityEngine;

[TaskCategory("Mod")]
public class GetEnemyEntityByGuid : Action
{
    public override TaskStatus OnUpdate()
    {
        m_entity = m_entity == null ? GameData.Instance.Enemies.Find(s => s.Guid == guid) : m_entity;
        output.Value = m_entity;
        return output.Value ? TaskStatus.Success : TaskStatus.Failure;
    }
    
    [SerializeField] [RequiredField] private string guid;
    private Entity m_entity;
    [SerializeField] [RequiredField] private SharedEntity output;
}
