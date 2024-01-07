using System.Collections;
using DG.Tweening;
using Thor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DemonUI : MonoBehaviour
{
    public Text text;

    private int _demon;
    
    private int _demonMax;
    
    [FormerlySerializedAs("m_content")] 
    [SerializeField]
    private RectTransform mContent;
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (var simulationPlayer in Game.Instance.Simulation.Players)
        {
            simulationPlayer.RegisterEvent(PlayerEvent.EventType.SpawnsAvatar, OnSpawnsAvatar);
            simulationPlayer.RegisterEvent(PlayerEvent.EventType.DestroysAvatar, OnDestroysAvatar);
        }
    }
    
    private Coroutine mAnimateCoroutine;
    private void OnSpawnsAvatar(PlayerEvent playerEvent)
    {
        Game.Instance.Simulation.RegisterEvent(SimulationEvent.EventType.ZoneLoadEnd, OnZoneLoadEnd);
        mContent.gameObject.SetActive(true);
        _demonMax = -1;
        if (mAnimateCoroutine != null)
        {
            StopCoroutine(mAnimateCoroutine);
        }
        mAnimateCoroutine = StartCoroutine(Animate());
    }
    
    private IEnumerator Animate()
    {
        var tweener = mContent.DOAnchorPos(new Vector2(150,0), 0.5f)
            .SetDelay(0.6f)
            .ChangeStartValue(Vector3.zero)
            .SetAutoKill(false)
            .Pause<Tweener>();
        yield return new WaitForSeconds(0.4f);
        tweener.Restart();
        mAnimateCoroutine = null;
        yield break;
    }
    private void OnDestroysAvatar(PlayerEvent playerEvent)
    {
        mContent.DOKill();
        mContent.anchoredPosition = Vector2.zero;
        Game.Instance.Simulation.UnregisterEvent(SimulationEvent.EventType.ZoneLoadEnd, OnZoneLoadEnd);
        mContent.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        _demon = GameData.Instance.GetUpgradeValue("demon");
        if (_demonMax < 0)
        {
            text.text = _demon.ToString();
        }
        else
        {
            text.text = _demon + "/" + _demonMax;
        }
    }
    
    private void OnZoneLoadEnd(SimulationEvent simEvent)
    {
        var zone = Game.Instance.Simulation.Zone;
        if (zone.Data.FloorNumber > 0 
            && zone.CurrentRoom.Encounter.HasTag("begin") 
            && (zone.Rooms.Exists(room => room.EncounterTags == "end") 
                || zone.Data.Mode != Game.GameMode.Rogue))
        {
            Random.State state;
            using (new Rand.Scope(Rand.StateType.Shoguul))
            {
                state = Random.state;
                _demonMax = Rand.RangeInclusive(0, 100);
            }
            Rand.SetState(Rand.StateType.Shoguul, state);
        }
        else
        {
            _demonMax = -1;
        }
        
    }
}
