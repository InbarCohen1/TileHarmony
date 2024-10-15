using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputController : Singleton<InputController>
{
    [SerializeField] private Button _shuffleButton;
    [SerializeField] private Button _undoButton;
    [SerializeField] private Button _removeButton;
    [SerializeField] private Button _boosterButton;
    [SerializeField] private Button _lockButton;

    private void Start()
    {
        _shuffleButton.onClick.AddListener(() => ToolManager.Instance.ActivateTool(ToolManager.ToolType.Shuffle));
        _undoButton.onClick.AddListener(() => ToolManager.Instance.ActivateTool(ToolManager.ToolType.Undo));
        _removeButton.onClick.AddListener(() => ToolManager.Instance.ActivateTool(ToolManager.ToolType.RemoveTile));
        _boosterButton.onClick.AddListener(() => ToolManager.Instance.ActivateTool(ToolManager.ToolType.Booster));
        _lockButton.onClick.AddListener(() => ToolManager.Instance.ActivateTool(ToolManager.ToolType.LockTile));
    }

    private void Update()
    {
        MovingLeft = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
        MovingRight = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
        MovingUp = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
        MovingDown = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
    }
    public bool MovingLeft { get; set; }
    public bool MovingRight { get; set; }
    public bool MovingDown { get; set; }
    public bool MovingUp { get; set; }

}
