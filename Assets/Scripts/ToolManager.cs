using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private ToolType _activeTool = ToolType.None;
    public ToolType ActiveTool => _activeTool;

    public void ActivateTool(ToolType tool)
    {
        _activeTool = tool;
        Debug.Log($"Active Tool is: + {tool}");
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

}
