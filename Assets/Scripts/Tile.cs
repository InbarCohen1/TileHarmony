using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public TileState _state { get; private set; }
    public TileCell _cell { get; private set; }
    public bool locked { get; set; }  // insures no multiple mergings

    public int _number { get; private set; } // TODO:rename ->value
    private Image _background;
    private TextMeshProUGUI _text; // TODO: just use Text instad of TextMeshPro

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _moveClip;
    [SerializeField] private AudioClip _mergeClip;

    private void Awake()
    {
        _background = GetComponent<Image>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void SetState(TileState state, int number)
    {
        _state = state;
        _number = number;

        _background.color = _state.backgroundColor;
        _text.color = _state.textColor;
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
        _audioSource.PlayOneShot(_moveClip);
    }

    //cell param is the one we merge to
    public void Merge(TileCell cell)
    {
        if (_cell != null)
        {
            _cell._tile = null;
        }

        _cell = null;
        cell._tile.locked = true; // disable merging to this tile in the current movement

        StartCoroutine(Animate(cell.transform.position, true));
        _audioSource.PlayOneShot(_mergeClip);
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

        if (merging)
        {
            Destroy(gameObject);
        }
    }
}
