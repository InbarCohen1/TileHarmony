using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    private bool _pressed;
    private bool _downThisFrame;
    private bool _upThisFrame;

    public bool Pressed => _pressed;
    public bool DownThisFrame => _downThisFrame;
    public bool UpThisFrame => _upThisFrame;

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = true;
       // _downThisFrame = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed = false;
        //_upThisFrame = true;
    }

    private void Update()
    {
        if (_downThisFrame)
        {
            _downThisFrame = false;
        }
        else if (_upThisFrame)
        {
            _upThisFrame = false;
        }
    }
}
