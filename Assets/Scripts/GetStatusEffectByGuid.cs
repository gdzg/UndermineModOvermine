using BehaviorDesigner.Runtime.Tasks;
using Thor;
using UnityEngine;

[TaskCategory("Mod")]
public class GetStatusEffectByGuid : Action
{
    public override TaskStatus OnUpdate()
    {
        m_entity = m_entity ? m_entity :GameData.Instance.StatusEffects.Find(s => s.Entity.Guid == guid).Entity;
        output.Value = m_entity;
        return output.Value ? TaskStatus.Success : TaskStatus.Failure;
    }
    
    [SerializeField] [RequiredField] private string guid;
    private Entity m_entity;
    [SerializeField] [RequiredField] private SharedEntity output;
}