using System.Collections;
using Rewired;
using Thor;
using UnityEngine;
using UnityEngine.UI;

public class MapTeleport : MonoBehaviour
{
    private MiniMapCell _cell;

    private Room _room;

    private Color _color;

    private Image _fill;
    private MiniMap _miniMap;
    private Color _currentColor;
    private Color _visitedColor;
    private Color _discoveredColor;


    // Start is called before the first frame update
    private void Start()
    {
        _cell = GetComponent<MiniMapCell>();
        _room = (Room)ExampleMod.GetFieldInfo<MiniMapCell>("mRoom").GetValue(_cell);
        _fill = (Image)ExampleMod.GetFieldInfo<MiniMapCell>("m_fill").GetValue(_cell);
        _miniMap = GetComponentInParent<MiniMap>();
        _currentColor = (Color)ExampleMod.GetFieldInfo<MiniMapCell>("m_currentColor").GetValue(_cell);
        _visitedColor = (Color)ExampleMod.GetFieldInfo<MiniMapCell>("m_visitedColor").GetValue(_cell);
        _discoveredColor = (Color)ExampleMod.GetFieldInfo<MiniMapCell>("m_discoveredColor").GetValue(_cell);
    }

    // Update is called once per frame
    public void Update()
    {
        var zone = Game.Instance.Simulation.Zone;
        var currentRoom = zone.CurrentRoom;
        if (
            _room == currentRoom
            || !_room.Visited
            || !_miniMap.gameObject.activeSelf
        )
        {
            return;
        }

        var primaryPlayer = Game.Instance.Simulation.PrimaryPlayer;
        if (primaryPlayer == null || Game.Instance.Simulation.IsPaused)
        {
            return;
        }

        var keyboard = primaryPlayer.Input.controllers.Keyboard;
        if (!primaryPlayer.Input.controllers.hasKeyboard || !primaryPlayer.InputEnabled) return;
        if (_fill.color == _currentColor 
            && keyboard.GetKeyDown(KeyCode.LeftAlt) 
            && !QuickMove.CheckStatus(primaryPlayer.Avatar) 
            && !(currentRoom.DoorState == Room.DoorStateType.Closed 
                 || zone.MovingRooms
                 || _room.Encounter.DoorType == DoorExt.DoorType.Hidden))
        {
            _fill.color = _visitedColor;
            StartCoroutine(Teleport(primaryPlayer.Avatar));
        }
        
        var vector = transform.position - Input.mousePosition;
        var f = _miniMap.CellSize / 2f;
        if (Mathf.Abs(vector.x) <= f && Mathf.Abs(vector.y) <= f)
        {
            _fill.color = _currentColor;
            return;
        }

        if (_cell.Visited)
        {
            _fill.color = _visitedColor;
            return;
        }

        if (_cell.Discovered || _cell.Revealed)
        {
            _fill.color = _discoveredColor;
        }
    }

    private IEnumerator Teleport(Entity entity)
    {
        Vector2Int vector = _room.Position - Game.Instance.Simulation.Zone.CurrentRoom.Position;
        int x = vector.x;
        if (!_room.Neighbors.TryGetValue(x > 0 ? Direction.West : Direction.East, out _))
        {
            x = 0;
        }

        int y = vector.y;
        if (!_room.Neighbors.TryGetValue(y > 0 ? Direction.South : Direction.North, out _))
        {
            y = 0;
        }

        Direction direction;
        if (Mathf.Abs(x) - Mathf.Abs(y) > 0)
        {
            direction = x > 0 ? Direction.West : Direction.East;
        }
        else
        {
            direction = y > 0 ? Direction.South : Direction.North;
        }

        Game.Instance.FxManager.Emit("explosion4_large_rev", entity.Position, Vector3.zero, 1f);
        yield return new WaitForSeconds(0.2f);
        Vector3 vector2 = _room.GetStartPosition(direction).GetLocalSpawnPosition(0);
        vector2 = new Vector3(vector2.x, 0f, vector2.z);
        Vector3 vector3 = entity.transform.localEulerAngles;
        Game.Instance.Simulation.Zone.SetCurrentRoom(Zone.RoomMoveType.Instant, _room, direction, vector2);
        Game.Instance.FxManager.Emit("explosion4_large", entity.Position, Vector3.zero, 1f);
        switch (direction)
        {
            case Direction.North:
                entity.transform.localEulerAngles = new Vector3(vector3.x, 180f, vector3.z);
                break;
            case Direction.South:
                entity.transform.localEulerAngles = new Vector3(vector3.x, 0f, vector3.z);
                break;
            case Direction.East:
                entity.transform.localEulerAngles = new Vector3(vector3.x, 270f, vector3.z);
                break;
            case Direction.West:
                entity.transform.localEulerAngles = new Vector3(vector3.x, 90f, vector3.z);
                break;
        }
    }
}