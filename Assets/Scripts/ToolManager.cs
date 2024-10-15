using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ToolManager;

public class ToolManager : Singleton<ToolManager>
{ 
    public enum ToolType
    {
        None,
        Shuffle,
        Undo,
        RemoveTile,
        Booster,
        LockTile
    }
    private Dictionary<ToolType, int> _cooldownDurations = new Dictionary<ToolType, int>
    {
        { ToolType.Shuffle, 5 },
        { ToolType.Undo, 3 },
        { ToolType.RemoveTile, 20 },
        { ToolType.LockTile, int.MaxValue }, 
        { ToolType.Booster, int.MaxValue }
    };

    private Dictionary<ToolType, int> _cooldowns = new Dictionary<ToolType, int>();

    private ToolType _activeTool = ToolType.None;
    public ToolType ActiveTool => _activeTool;

    private void Start()
    {
        foreach (ToolType toolType in System.Enum.GetValues(typeof(ToolType)))
        {
            if (toolType != ToolType.None)
            {
                _cooldowns[toolType] = 0;
            }
        }
    }

    public void ActivateTool(ToolType toolType)
    {
        if (CanUseTool(toolType))
        {
            _activeTool = toolType;
            Debug.Log($"Active Tool is: + {toolType}");
        }
    }

    public void DecreaseCooldowns()
    {
        List<ToolType> keys = new List<ToolType>(_cooldowns.Keys);
        foreach (var key in keys)
        {
            if (_cooldowns[key] > 0)
            {
                _cooldowns[key]--;
            }
        }
    }

    public void DeactivateTool()
    {
        Debug.Log($"{_activeTool} tool Mode is off");

        _cooldowns[_activeTool] = _cooldownDurations[_activeTool];
        _activeTool = ToolType.None;
    }

    public bool IsToolActive(ToolType tool) //TODO:Remove
    {
        return _activeTool == tool;
    }

    public bool IsAnyToolActive()
    {
        return _activeTool != ToolType.None;
    }

    public bool CanUseTool(ToolType toolType)
    {
        if (_cooldowns.ContainsKey(toolType) && _cooldowns[toolType] > 0)
        {
            return false;
        }

        foreach (var cooldown in _cooldowns.Values)
        {
            if (cooldown > 0)
            {
                return false; 
            }
        }

        return true;
    }
    public void ResetCooldowns()
    {
        List<ToolType> keys = new List<ToolType>(_cooldowns.Keys);
        foreach (var key in keys)
        {
            _cooldowns[key] = 0;
        }
    }

    public ICommand GetCommand(TileBoard tileBoard)
    {
        switch (_activeTool)
        {
            case ToolType.Undo:
                return new UndoCommand(tileBoard);
            case ToolType.Shuffle:
                return new ShuffleCommand(tileBoard);
            case ToolType.RemoveTile:
                return new RemoveTileCommand(tileBoard);
            case ToolType.LockTile:
                return new LockTileCommand(tileBoard);
            case ToolType.Booster:
                return new BoosterCommand(tileBoard);
            default:
                return null;
        }
    }

}

public abstract class TileToolCommand : ICommand
{
    public bool Execute()
    {
        //Tile tile = InputController.Instance.GetComponentAtMousePosition<Tile>(TileBoard.TilesLayerName);
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10.0f)); 
        worldPosition.z = 0;

        //Collider2D hitCollider = Physics2D.OverlapPoint(worldPosition);
        Collider2D hitCollider = Physics2D.OverlapPoint(worldPosition, LayerMask.GetMask(TileBoard.TilesLayerName));

        if (hitCollider != null)
        {
            Tile tile = hitCollider.GetComponent<Tile>();
            if (tile != null)
            {
                ExecuteLogic(tile);
                return true;
            }
            else
            {
                Debug.Log("No Tile component found on the hit object.");
            }
        }
        else
        {
            Debug.Log("No collider hit.");
        }
        return false;
    }

    protected abstract void ExecuteLogic(Tile tile);
}

