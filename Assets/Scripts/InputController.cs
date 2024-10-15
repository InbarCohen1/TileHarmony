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
        //MovingDown = Input.GetKey(KeyCode.UpArrow) || (_moveDownButton != null ? _moveDownButton.Pressed : false);
    }
    public bool MovingLeft { get; set; }
    public bool MovingRight { get; set; }
    public bool MovingDown { get; set; }
    public bool MovingUp { get; set; }

    public T GetComponentAtMousePosition<T>(string layerMaskName) where T : class
    {
        int layer = LayerMask.NameToLayer(layerMaskName);
        if (layer == -1)
        {
            Debug.LogError($"Layer '{layerMaskName}' does not exist.");
            return null;
        }
        LayerMask layerMask = 1 << layer;

        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10.0f));
        worldPosition.z = 0;

        Collider2D hitCollider = Physics2D.OverlapPoint(worldPosition, layerMask);

        if (hitCollider != null)
        {
            return hitCollider.GetComponent<T>();
        }

        return null;
    }

}
