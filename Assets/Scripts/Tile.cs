using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public TileState _state { get; private set; }
    public TileCell _cell { get; private set; }
    public int _number { get; private set; } // TODO:rename ->value

    private Image _background;
    private TextMeshProUGUI _text; // TODO: just use Text instad of TextMeshPro


    private void Awake()
    {
        _background = GetComponent<Image>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void SetState(TileState state, int number)
    {
        _state = state;
        _number = number;

        _background.color = _state._backgroundColor;
        _text.color = _state._textColor;
        _text.text = _number.ToString();
    }

    public void Spawn(TileCell cell)
    {
        if(_cell is not null)
        {
            _cell._tile = null;
        }

        _cell = cell;
        _cell._tile = this;

        transform.position = _cell.transform.position;
    }

    public void MoveTo(TileCell cell)
    {
        if (_cell is not null)
        {
            _cell._tile = null;
        }

        _cell = cell;
        _cell._tile = this;

        StartCoroutine(Animate(cell.transform.position, false));
    }

    private IEnumerator Animate(Vector3 to, bool merging) //TODO: renaming
    {
        float elapsed = 0f;
        float duration = 0.1f;

        Vector3 from = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = to;

        //if (merging)
        //{
        //    Destroy(gameObject);
        //}
    }
}
