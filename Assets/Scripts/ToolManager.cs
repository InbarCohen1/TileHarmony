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
        { ToolType.LockTile, int.MaxValue }, // Once a game
        { ToolType.Booster, int.MaxValue } // Once a game
    };

    private Dictionary<ToolType, int> _cooldowns = new Dictionary<ToolType, int>();


    private ToolType _activeTool = ToolType.None;
    public ToolType ActiveTool => _activeTool;

    public void ActivateTool(ToolType toolType)
    {
        if (CanUseTool(toolType))
        {
            _activeTool = toolType;
        Debug.Log($"Active Tool is: + {toolType}");
            // Set the cooldown
            _cooldowns[toolType] = _cooldownDurations[toolType];
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
        _activeTool = ToolType.None;
    }

    public bool IsToolActive(ToolType tool)
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
                return false; // Another tool is in cooldown
            }
        }

        return true;
    }
    public void ResetCooldowns()
    {
        _cooldowns.Clear();
    }
}
