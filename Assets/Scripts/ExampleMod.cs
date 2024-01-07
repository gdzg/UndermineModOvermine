using System.Reflection;
using Overmine.API;
using Overmine.API.Events;
using Overmine.API.Extensions;
using Thor;
using UnityEngine;

public class ExampleMod : Mod
{
    public const BindingFlags BindingFlags = System.Reflection.BindingFlags.Instance |
                                             System.Reflection.BindingFlags.Public |
                                             System.Reflection.BindingFlags.NonPublic;

    public static ExampleMod Instance { get; private set; }

    public ExampleMod(ModResources resources) : base(resources)
    {
        Instance = this;
        GameEvents.Register<SetupEvent.Post>(OnSetup);
    }

    private void OnSetup(SetupEvent.Post ev)
    {
        Resources.ResolveReferences();
        var rogueModeHpScale = GetFieldInfo<ZoneData>("m_rogueModeHPScale");
        var rogueModeDamageScale = GetFieldInfo<ZoneData>("m_rogueModeDamageScale");
        ev.Instance.Zones.ForEach(zone =>
        {
            if (zone.name.Contains("OM_") && zone.FloorNumber > 6 && zone.Mode == Game.GameMode.Rogue)
            {
                zone.CostModifiers.ForEach(cost => cost.operation.value.Value = zone.FloorNumber > 9 ? 3f : 2f);
                rogueModeHpScale?.SetValue(zone, zone.RogueModeHPScale * (zone.FloorNumber > 9 ? 2f : 1.5f));
                rogueModeDamageScale?.SetValue(zone, zone.RogueModeDamageScale * (zone.FloorNumber > 9 ? 2f : 1.5f));
            }
        });
        var content = GameObject.Find("Game/UI Canvas/UIContainer/HUD/Content/ResourcePanel/Content");
        var gameObject = Object.Instantiate(Resources.LoadAsset<GameObject>("DemonUI"), content.transform, false);
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1025, 220);
        var map = GameObject.Find("Game/UI Canvas/UIContainer/HUD/Content/MiniMap");
        ((MiniMapCell)GetFieldInfo<MiniMap>("m_cellPrefab").GetValue(map.GetComponent<MiniMap>()))
            .gameObject.AddComponent<MapTeleport>();
        ev.Instance.RegisterAll(Resources);
    }

    public static FieldInfo GetFieldInfo<T>(string name)
    {
        return typeof(T).GetField(name, BindingFlags);
    }
}