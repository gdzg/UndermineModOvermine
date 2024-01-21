using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;
using Overmine.API;
using Overmine.API.Assets;
using Overmine.API.Events;
using Overmine.API.Extensions;
using Thor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Overmine.API.Events.SetupEvent;
using Object = UnityEngine.Object;

public class ExampleMod : Mod
{
    public const BindingFlags BindingFlags = System.Reflection.BindingFlags.Instance |
                                             System.Reflection.BindingFlags.Public |
                                             System.Reflection.BindingFlags.NonPublic;

    public static ExampleMod Instance { get; private set; }

    public ModData ModData;

    public ExampleMod(ModResources resources) : base(resources)
    {
        Instance = this;
        GameEvents.Register<Post>(OnSetup);
        GameEvents.Register<RoomEvent.Setup.Post>(OnRoomEventSetupPost);
        GameEvents.Register<GameLoadEvent>(OnGameLoad);
        GameEvents.Register<CollectorRoomEvent.PopulateFamiliars.Pre>(OnCollectorRoomPrePopulateFamiliars);
        GameEvents.Register<GameConditionCheckEvent.Pre>(OnGameConditionCheckEventPre);
    }

    private void OnRoomEventSetupPost(RoomEvent.Setup.Post post)
    {
        var encounter = post.Room.Encounter;
        if (encounter != null && encounter.HasTag("antechamber") && encounter.name == "StartRunEncounter")
        {
            Object.Instantiate(ModData.npc.gameObject, encounter.transform).transform.localPosition =
                new Vector3(11.6f, 0, 0);
        }
    }

    private void OnGameConditionCheckEventPre(GameConditionCheckEvent.Pre pre)
    {
        var gameCondition = pre.condition;
        var requirements = GetFieldValue<List<Requirement>, GameCondition>("m_requirements", gameCondition);
        if (requirements.Exists(x => x.type == Requirement.RequirementType.ShopItemCount)
            && Requirement.CheckAll(requirements)
            && Game.Instance.Simulation.PrimaryPlayer.Avatar
                .GetComponent<PetOwnerExt>()
                .GetPet("75742aa2-4736-4b82-8369-1b3814621c04")?.GetExtension<PetExt>()?.CurrentLevel >= 2)
        {
            foreach (var vector in ModData.shopMarketVectors)
            {
                Object.Instantiate(ModData.shopMarket, gameCondition.transform, true).transform.localPosition = vector;
            }
        }
    }

    private void OnCollectorRoomPrePopulateFamiliars(CollectorRoomEvent.PopulateFamiliars.Pre pre)
    {
        var collectorRoom = pre.Room;
        if (GetFieldValue<int, CollectorRoom>("mVisitCount", collectorRoom) != 0)
        {
            return;
        }

        var sprites = collectorRoom.transform.GetChild(7).GetComponentsInChildren<SpriteRenderer>()
            .Select(x => x.sprite)
            .ToList();
        foreach (var spriteRenderer in ModData.collectorRoomFamiliar.GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderer.sprite = sprites.Find(x => x.name == spriteRenderer.sprite.name);
        }

        var markers = GetFieldValue<List<Entity>, CollectorRoom>("m_markers", collectorRoom);
        foreach (var vector in ModData.collectorRoomFamiliarVectors)
        {
            var instantiate = Object.Instantiate(ModData.collectorRoomFamiliar, collectorRoom.transform, true);
            instantiate.transform.localPosition = vector;
            var children = instantiate.GetComponentsInChildren<Entity>();
            for (var index = 0; index < children.Length; index++)
            {
                var entity = children[index];
                entity.Initialize();
                if (index == 2)
                {
                    markers.Add(entity);
                }
            }
        }
    }

    private void OnGameLoad(GameLoadEvent post)
    {
        PoolInit();

        foreach (var entity in UnityEngine.Resources.FindObjectsOfTypeAll<Entity>())
        {
            if (entity.name == "RockMimic")
            {
                entity.GetComponent<BehaviorExt>().Behaviors[5] = ModData.rockMimicBegin;
                continue;
            }

            if (entity.Guid == "565c99ae57604792aea578086581d24d")
            {
                var data = entity.GetComponents<ExtendedBehaviorTree>()[0].GetBehaviorSource().TaskData;
                var unityObjects = data.fieldSerializationData.unityObjects;
                var rockMimic1 = unityObjects[0];
                var rockMimic2 = unityObjects[1];
                unityObjects.Insert(1, rockMimic2);
                unityObjects.Insert(1, rockMimic2);
                unityObjects.Insert(7, rockMimic1);
                unityObjects.Insert(7, rockMimic1);
                data.JSONSerialization = ModData.startMimics.BehaviorSource.TaskData.JSONSerialization;
            }
        }
    }

    private void OnSetup(Post ev)
    {
        Resources.ResolveReferences();
        ModData = Resources.LoadAsset<ModData>("Mod Data");
        ZoneInit(ev);
        UIInit();
        var gameData = ev.Instance;
        gameData.RegisterAll(Resources);

        var hpTierData = GetFieldValue<List<EnemyData.HPTierData>, GameData>("m_hpTiers", gameData)
            .Find(x => x.hpTier == EnemyData.HPTierType.Boss);
        hpTierData.hp = hpTierData.hp / 2 + hpTierData.hp;
        var monkeyPetExt = gameData.Familiars.Find(x => x.Entity.Guid == "35dcfe88c6114469a583c3123a1049e0");
        var spawn = ((FamiliarData)monkeyPetExt.Entity.Data).Skins[0].Animation.Animations
            .Find(x => x.key == "Spawn").clip;
        foreach (var petExt in Resources.LoadAllObjectsWithComponent<PetExt>())
        {
            var entity = petExt.GetComponent<Entity>();
            if (entity.Guid != "")
            {
                ((FamiliarData)petExt.Entity.Data).Skins[0].Animation.Animations[5].clip = spawn;
            }

            //icePetExt
            if (entity.Guid == "e49a0381-fda5-4115-b807-be5cfafc3e73")
            {
                petExt.GetComponent<BehaviorExt>().Behaviors.Add(monkeyPetExt.GetComponent<BehaviorExt>().Behaviors[3]);
            }
        }
        var fieldInfo = GetFieldInfo<BehaviorExt>("m_externalBehaviors");
        foreach (var behaviorExt in UnityEngine.Resources.FindObjectsOfTypeAll<BehaviorExt>()
                     .Where(x => x.name == "LargeCrystalProjectile"))
        {
            fieldInfo.SetValue(behaviorExt, ModData.largeCrystalProjectileHit);
        }
        SupportOldMod(ev);
    }

    private static void SupportOldMod(Post ev)
    {
        if (!(Assembly.Load("UnderMine").GetType("Thor.Utils")
                    ?.GetField("Prefab", BindingFlags.Static | BindingFlags.Public)?.GetValue(null) is
                Dictionary<string, Entity> dictionary)) return;
        dictionary["PracticeDummy"].GetComponent<HealthExt>().Invulnerable = true;
        dictionary[ev.Instance.GetEquipment("ae38d3a1-fc5c-4a38-ab57-0eabc0055dbe").Entity.name] =
            GameData.Instance.Enemies.Find(entity => entity.Guid == "523009b8775f471ea28ef583c40bb77f");
    }

    private void PoolInit()
    {
        var fieldInfo = GetFieldInfo<BehaviorExt>("m_externalBehaviors");
        var dictionary =
            GetFieldValue<Dictionary<string, List<Entity>>, PoolManager>("mDynamicPool", Game.Instance.PoolManager);
        foreach (var entity in dictionary["b3139e273cac4821a404e19355bbd0c3"])
        {
            fieldInfo.SetValue(entity.GetComponent<BehaviorExt>(), ModData.largeCrystalProjectileHit);
        }

        PoolOldSupport(dictionary);
    }

    private static void PoolOldSupport(Dictionary<string, List<Entity>> dictionary)
    {
        var showDamageKicker = GetFieldInfo<HealthExt>("m_showDamageKicker");
        var maxHp = GetFieldInfo<HealthExt>("m_maxHP");
        var showInUI = GetFieldInfo<HealthExt>("m_showInUI");
        foreach (var entity in dictionary["8f3e7b048de04b0a8614767c08724baa"])
        {
            var component = entity.GetComponent<HealthExt>();
            showDamageKicker.SetValue(component, true);
            maxHp.SetValue(component, 5);
            showInUI.SetValue(component, true);
        }

        var item = GameData.Instance.Enemies.Find(x => x.Data.guid == "bb484a5832f24197a2ec9274c07e6446")
            .GetComponent<BehaviorExt>().Behaviors[3];
        var armor = GetFieldInfo<HealthExt>("m_armor");
        foreach (var entity in dictionary["a7f0a079cf044f8a9ad5bc09c3088742"])
        {
            var armors = GetFieldValue<List<HealthExt.Armor>>(armor, entity.GetComponent<HealthExt>());
            armors.Add(new HealthExt.Armor
            {
                id = "7b64977e-0dbd-4754-9c5b-55f709aba878",
                armorType = HealthExt.Armor.ArmorType.StatusTag,
                tag = "ignite"
            });
            armors.Add(new HealthExt.Armor
            {
                id = "5479e0ae-a6a1-49f1-888c-ae4bf8b5dd72",
                percent = 1f,
                damageType = HealthExt.DamageType.Fire,
                armorType = HealthExt.Armor.ArmorType.DamageType
            });
            entity.GetComponent<BehaviorExt>().Behaviors.Add(item);
        }
    }

    private void ZoneInit(Post ev)
    {
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
    }

    private void UIInit()
    {
        var content = GameObject.Find("Game/UI Canvas/UIContainer/HUD/Content/ResourcePanel/Content");
        Object.Instantiate(ModData.demonUI.gameObject, content.transform, false)
            .GetComponent<RectTransform>().anchoredPosition = new Vector2(-1025, 220);
        GetFieldValue<MiniMapCell, MiniMap>("m_cellPrefab",
            GameObject.Find("Game/UI Canvas/UIContainer/HUD/Content/MiniMap").GetComponent<MiniMap>()
        ).gameObject.AddComponent<MapTeleport>();
    }


    public static FieldInfo GetFieldInfo<T>(string name)
    {
        return typeof(T).GetField(name, BindingFlags);
    }

    public static T GetFieldValue<T>(FieldInfo fieldInfo, object obj)
    {
        return (T)fieldInfo.GetValue(obj);
    }

    public static T GetFieldValue<T, S>(string name, S obj)
    {
        return GetFieldValue<T>(GetFieldInfo<S>(name), obj);
    }
}