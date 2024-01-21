using System.Collections;
using System.Collections.Generic;
using Overmine.API.Assets;
using UnityEngine;
using Thor;

[CreateAssetMenu(fileName = "Mod Data", menuName = "Data/Mod Data")]
public class ModData : ScriptableObject
{
    public BehaviourGraph startMimics;

    public DemonUI demonUI;

    public BehaviourGraph rockMimicBegin;

    public BehaviorContainer largeCrystalProjectileHit;

    public GameObject collectorRoomFamiliar;

    public Vector3[] collectorRoomFamiliarVectors;
    
    public GameObject shopMarket;

    public Vector3[] shopMarketVectors;

    public Entity npc;
}
