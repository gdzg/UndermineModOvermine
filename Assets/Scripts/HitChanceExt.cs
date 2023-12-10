using System;
using Thor;
using UnityEngine;

public class HitChanceExt : Extension
{
    [Restorable] [SerializeField] private bool m_enabled = true;
    [Restorable] [SerializeField] private int m_priority;
    [Restorable] [SerializeField] private float m_chance;
    [Restorable] [SerializeField] private bool m_showImmuneKicker = true;

    [BehaviorProperty]
    public bool Enabled
    {
        get => m_enabled;
        set => m_enabled = value;
    }

    public override void Setup()
    {
        Owner.RegisterEvent(EntityEvent.EventType.Hit, OnHit, m_priority);
    }

    public override void Shutdown()
    {
        Owner.UnregisterEvent(EntityEvent.EventType.Hit, OnHit);
    }

    private void OnHit(Entity target, EntityEvent entityEvent)
    {
        EntityEvent.Hit hitEvent = entityEvent as EntityEvent.Hit;
        if (Entity.IsDisabled || !m_enabled || hitEvent.DamageAmount <= 0)
            return;
        if (Rand.Chance(m_chance))
        {
            hitEvent.DamageAmount = 0;
            Entity.FireEvent(EntityEvent.Proc.Create(Entity.Position));
            Owner.GetExtension<AudioExt>().PlayOneShot(Proc.kPositiveAudio);
            if (m_showImmuneKicker)
                ShowImmuneKicker();
        }
    }

    private void ShowImmuneKicker()
    {
        Vector3 position = Entity.Position;
        VisualExt extension = Entity.GetExtension<VisualExt>();
        if (extension != null)
            position = extension.Position;
        Game.Instance.PopupManager.CreatePopup(PopupManager.GetPopupPrefab("ImmunePopup"), null, Entity.kNone, position);
    }


    public override Action<Component> Capture()
    {
        bool enabled = m_enabled;
        int priority = m_priority;
        bool showImmuneKicker = m_showImmuneKicker;
        float m_chance = this.m_chance;
        return component =>
        {
            HitChanceExt shieldExt = component as HitChanceExt;
            shieldExt.m_enabled = enabled;
            shieldExt.m_priority = priority;
            shieldExt.m_showImmuneKicker = showImmuneKicker;
            shieldExt.m_chance = m_chance;
        };
    }
}