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
        MovingLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        MovingRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        MovingUp = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
        MovingDown = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
    }
    public bool MovingLeft { get; set; }
    public bool MovingRight { get; set; }
    public bool MovingDown { get; set; }
    public bool MovingUp { get; set; }

}
