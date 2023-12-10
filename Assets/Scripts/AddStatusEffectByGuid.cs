using BehaviorDesigner.Runtime.Tasks;
using Thor;
using UnityEngine;

[TaskCategory("Mod")]
public class AddStatusEffectByGuid : Action
{
    [RequiredField] [SerializeField] private SharedEntity m_target;
    [SerializeField] private StatusEffectExt.StatusEffectData m_data = new StatusEffectExt.StatusEffectData();
    [RequiredField] [SerializeField] private SharedEntity m_source;

    public override TaskStatus OnUpdate()
    {
        var extension = m_target.Value.GetExtension<HealthExt>();
        if (extension == null)
            return TaskStatus.Failure;
        if (m_statusEffect == null)
        {
            var entity = GameData.Instance.StatusEffects.Find(s => s.Entity.Guid == m_statusEffect_guid)?.Entity;
            if (entity == null)
                return TaskStatus.Failure;
            m_statusEffect = entity.GetComponent<StatusEffectExt>();
            if (m_statusEffect == null)
                return TaskStatus.Failure;
        }
        switch (extension.AddStatusEffect(m_statusEffect, m_data, m_source.Value))
        {
            case HealthExt.AddStatusEffectResult.Success:
            case HealthExt.AddStatusEffectResult.Absorbed:
            case HealthExt.AddStatusEffectResult.Blocked:
            case HealthExt.AddStatusEffectResult.NonStacking:
                return TaskStatus.Success;
            default:
                return TaskStatus.Failure;
        }
    }

    [SerializeField] [RequiredField] private string m_statusEffect_guid;
    private StatusEffectExt m_statusEffect;
}