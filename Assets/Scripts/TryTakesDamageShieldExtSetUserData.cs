using BehaviorDesigner.Runtime.Tasks;
using Thor;
using UnityEngine;

[TaskCategory("Mod")]
public class TryTakesDamageShieldExtSetUserData : Action
{ 
    [RequiredField] [SerializeField] private SharedEntity source;
    [RequiredField] [SerializeField] private SharedEntity target;
    public override TaskStatus OnUpdate()
    {
        if (source.Value.UserData > -1)
        {
            return TaskStatus.Success;
        }

        var tryTakesDamageShieldExt = source.Value.GetExtension<TryTakesDamageShieldExt>();
        
        if (!tryTakesDamageShieldExt.Enabled)
        {
            return TaskStatus.Success;
        }
        var inventoryExt = target.Value.GetExtension<InventoryExt>();
        var mData = tryTakesDamageShieldExt.m_data;
        var mMax = tryTakesDamageShieldExt.m_max;
        var resource = inventoryExt.GetResource(mData);
        var minResource = inventoryExt.GetMinResource(mData);
        int changResource = resource - minResource;
        var mPercent = tryTakesDamageShieldExt.m_percent;
        if (changResource >= mPercent * mMax)
        {
            changResource = mPercent * mMax;
        }
        source.Value.UserData = changResource / mPercent;
        inventoryExt.ChangeResource(mData, -changResource, null, false, target.Value);
        return TaskStatus.Success;
    }
}
