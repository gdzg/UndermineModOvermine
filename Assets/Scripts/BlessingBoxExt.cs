using System;
using UnityEngine;
using Thor;

public class BlessingBoxExt : Extension
{
    private const string Hint = "blessing_box";

    [SerializeField] private int limit;

    [SerializeField] private int priority;

    public override void Setup()
    {
        Owner.RegisterEvent(EntityEvent.EventType.TryGainsStatusEffect, OnTryGainsStatusEffect, priority);
    }

    public override void Shutdown()
    {
        Owner.UnregisterEvent(EntityEvent.EventType.TryGainsStatusEffect, OnTryGainsStatusEffect);
    }


    private void OnTryGainsStatusEffect(Entity entity, EntityEvent entityEvent)
    {
        var tryGainsStatusEffect = entityEvent as EntityEvent.TryGainsStatusEffect;
        var effectPrefab = tryGainsStatusEffect.StatusEffectPrefab;
        var statusEffectExt = effectPrefab.GetComponent<StatusEffectExt>();

        var statusEffectData = tryGainsStatusEffect.Data;

        // 自身被禁用,状态已经被block,不是祝福,不是自身添加的祝福
        if (Entity.IsDisabled
            || statusEffectData.blocked
            || Hint.Equals(statusEffectData.hint?.Value)
            || !statusEffectExt.IsBlessing) return;
        
        statusEffectData.blocked = true;
        //显示数量,记录id,UserString会被存档保存
        Entity.UserString += $":{effectPrefab.Guid}";
        Entity.UserData++;
        if (Entity.UserData < limit) return;
        var healthExt = Owner.GetExtension<HealthExt>();
        //移除自身
        healthExt.RemoveStatusEffect(Entity, true, true, out _);
        //添加祝福
        foreach (var guid in Entity.UserString.Split(':'))
        {
            if (string.Empty.Equals(guid)) continue;
            const int level = 2;
            var data = new StatusEffectExt.StatusEffectData(level, -1, -1f, 0, string.Empty, Hint, false);
            healthExt.AddStatusEffect(GameData.Instance.GetStatusEffect(guid), data, null);
        }

        //获取随机的圣物
        var itemData = GameData.Instance
            .GetLootTable(ItemData.ItemHint.Relic)
            .GetItem(item => item.CanDrop(out _));
        //获取圣物的模版
        var itemTemplate = GameData.Instance.GetItemTemplate(itemData);

        //掉落圣物
        ItemExt.ScatterItem(itemTemplate, itemData, Owner.Position);
    }

    public override Action<Component> Capture()
    {
        return component =>
        {
            var blessingBoxExt = component as BlessingBoxExt;
            blessingBoxExt.priority = priority;
            blessingBoxExt.limit = limit;
        };
    }
}