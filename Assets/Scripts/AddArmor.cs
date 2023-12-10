using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Thor;
using UnityEngine;

[TaskCategory("Mod")]
public class AddArmor : Action
  {
    [RequiredField]
    [SerializeField]
    private SharedEntity m_target;
    [SerializeField]
    private HealthExt.Armor m_armor;
    [SerializeField]
    private bool m_removeOnComplete;
    [RequiredField]
    [SerializeField]
    private SharedFloat m_value;

    public override TaskStatus OnUpdate()
    {
      HealthExt extension = m_target.Value.GetExtension<HealthExt>();
      if (extension == null)
        return TaskStatus.Failure;
      m_armor.percent = m_value.Value;
      extension.AddArmor(m_armor);
      return TaskStatus.Success;
    }

    public override void OnBehaviorRestart()
    {
      if (!m_removeOnComplete)
        return;
      Remove();
    }

    public override void OnBehaviorComplete()
    {
     
        if (!m_removeOnComplete)
          return;
        Remove();
    }

    private void Remove()
    {
      if (m_target.Value == null)
        return;
      var extension = m_target.Value.GetExtension<HealthExt>();
      if (extension == null)
        return;
      extension.RemoveArmor(m_armor.id);
    }
  }