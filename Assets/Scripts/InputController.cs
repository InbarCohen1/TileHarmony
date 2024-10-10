using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputController : Singleton<InputController>
{
    //[SerializeField] private CanvasButton _moveRightButton;
    //[SerializeField] private CanvasButton _moveLeftButton;
    //[SerializeField] private CanvasButton _moveDownButton;
    //[SerializeField] private CanvasButton _moveUpButton;

    [SerializeField] private Button _removeButton;
    [SerializeField] private Button _boosterButton;
    [SerializeField] private Button _lockButton;
    private bool _isBoosterActive = false;
    public bool IsBoosterActive => _isBoosterActive;

    private void Start()
    {
        _removeButton.onClick.AddListener(OnRemoveButtonClick);
        _boosterButton.onClick.AddListener(OnBoosterButtonClick);
        _lockButton.onClick.AddListener(OnLockButtonClick);
    }

    private void Update()
    {
        MovingLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        MovingRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        MovingUp = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
        MovingDown = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        //MovingDown = Input.GetKey(KeyCode.UpArrow) || (_moveDownButton != null ? _moveDownButton.Pressed : false);
    }
    public bool MovingLeft { get; set; }

    public bool MovingRight { get; set; }
    public bool MovingDown { get; set; }
    public bool MovingUp { get; set; }

    private void OnRemoveButtonClick()
    {
        ToolManager.Instance.ActivateTool(ToolManager.ToolType.RemoveTile);
    }

    public void OnBoosterButtonClick()
    {
        ToolManager.Instance.ActivateTool(ToolManager.ToolType.Booster);
    }

    public void OnLockButtonClick()
    {
        ToolManager.Instance.ActivateTool(ToolManager.ToolType.LockTile);
    }

}
