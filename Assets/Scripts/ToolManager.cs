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
        Freeze
    }

    private ToolType _activeTool = ToolType.None;

    public void ActivateTool(ToolType tool)
    {
        _activeTool = tool;
        Debug.Log($"Active Tool is: + {tool}");
    }
    public void DeactivateTool()
    {
        _activeTool = ToolType.None;
        Debug.Log($"Tool Mode is off");
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
