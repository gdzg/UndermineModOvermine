using System;
using Thor;
using UnityEngine;

public class TryTakesDamageShieldExt : Extension
{
    [Restorable] [SerializeField] private bool m_enabled = true;
    [Restorable] [SerializeField] private int m_priority;
    [Restorable] [SerializeField] private bool m_showImmuneKicker = true;
    [Restorable] [SerializeField] public int m_percent;
    [Restorable] [SerializeField] public int m_max;
    [Restorable] [SerializeField] public ResourceData m_data;

    [BehaviorProperty]
    public bool Enabled
    {
        get => m_enabled;
        set => m_enabled = value;
    }

    public override void Setup()
    {
        Owner.RegisterEvent(EntityEvent.EventType.TryTakesDamage, OnTryTakesDamage, m_priority);
    }

    public override void Shutdown()
    {
        Owner.UnregisterEvent(EntityEvent.EventType.TryTakesDamage, OnTryTakesDamage);
    }

    private void OnTryTakesDamage(Entity target, EntityEvent entityEvent)
    {
        var hitEvent = entityEvent as EntityEvent.TryTakesDamage;
        var userData = Entity.UserData;
        if (Entity.IsDisabled || !m_enabled || hitEvent.DamageAmount <= 0 || userData <= 0)
            return;
        var healthExt = Owner.GetExtension<HealthExt>();
        if (healthExt.NormalizeDamage || healthExt.CurrentHP > hitEvent.DamageAmount)
        {
            return;
        }

        var num = healthExt.CurrentHP + userData - hitEvent.DamageAmount;
        if (num <= 0) return;
        
        num = userData - hitEvent.DamageAmount;
        if (num >= 0)
        {
            Entity.UserData = num;
            hitEvent.DamageAmount = 0;
            Entity.FireEvent(EntityEvent.Proc.Create(Entity.Position));
            Owner.GetExtension<AudioExt>().PlayOneShot(Proc.kPositiveAudio);
            if (m_showImmuneKicker)
                ShowImmuneKicker();
        }
        else
        {
            hitEvent.DamageAmount -= userData;
            Entity.UserData = 0;
            Entity.FireEvent(EntityEvent.Proc.Create(Entity.Position));
            Owner.GetExtension<AudioExt>().PlayOneShot(Proc.kPositiveAudio);
        }
    }

    private void ShowImmuneKicker()
    {
        var position = Entity.Position;
        var extension = Entity.GetExtension<VisualExt>();
        if (extension != null)
            position = extension.Position;
        Game.Instance.PopupManager.CreatePopup(PopupManager.GetPopupPrefab("ImmunePopup"), null, Entity.kNone,
            position);
    }


    public override Action<Component> Capture()
    {
        var enabled = m_enabled;
        var priority = m_priority;
        var showImmuneKicker = m_showImmuneKicker;
        var mPercent = m_percent;
        var mMax = m_max;
        var mData = m_data;
        return component =>
        {
            var shieldExt = component as TryTakesDamageShieldExt;
            shieldExt.m_enabled = enabled;
            shieldExt.m_priority = priority;
            shieldExt.m_showImmuneKicker = showImmuneKicker;
            shieldExt.m_percent = mPercent;
            shieldExt.m_data = mData;
            shieldExt.m_max = mMax;
        };
    }
}