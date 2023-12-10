using BehaviorDesigner.Runtime.Tasks;
using Thor;
using UnityEngine;

[TaskCategory("Mod")]
public class SetGameOption : Conditional
{
    public override TaskStatus OnUpdate()
    {
        switch (m_option)
        {
            case CompareGameOption.GameOption.FullHealthPickup:
                GameData.Instance.FullHealthPickup = m_value;
                break;
            case CompareGameOption.GameOption.SpeedBoots:
                GameData.Instance.SpeedBoots = m_value;
                break;
        }
        return TaskStatus.Success;
    }

    [SerializeField] [RequiredField] public CompareGameOption.GameOption m_option;
    [RequiredField] [SerializeField] public bool m_value;
}