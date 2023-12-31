﻿using BehaviorDesigner.Runtime.Tasks;
using Thor;
using UnityEngine;

[TaskCategory("Mod")]
public class GetRandomItemByLootTable : Action
{
    public override TaskStatus OnUpdate()
    {
        m_Data = m_Data ?? GameData.Instance.LootTables.Find(lootTableData =>  lootTableData.guid == guid);
        var item = m_Data.GetItem(itemData => itemData.CanDrop(out _));
        output.Value = item;
        if (item != null)
        {
            if (incrementDropCount)
            {
                item.DropCount++;
            }
            return TaskStatus.Success;
        }
        
        return TaskStatus.Failure;
    }
    
    [SerializeField] [RequiredField] private string guid;
    private LootTableData m_Data;
    [SerializeField] [RequiredField] private SharedDataObject output;
    
    [SerializeField] [RequiredField] private bool incrementDropCount;
}