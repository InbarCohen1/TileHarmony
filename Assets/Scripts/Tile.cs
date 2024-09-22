using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public TileState State { get; private set; }
    public TileCell Cell { get; private set; }
    public bool IsLocked { get; set; }  // Insures no multiple mergings

    private Image _background;
    private TextMeshProUGUI _text; 

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _moveClip;
    [SerializeField] private AudioClip _mergeClip;

    private void Awake()
    {
        _background = GetComponent<Image>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void SetState(TileState newState)
    {
        State = newState;

        _background.color = State.backgroundColor;
        _text.color = State.textColor;
        _text.text = newState.number.ToString();
    }

    public void Spawn(TileCell cell)
    {
        if (Cell != null)
        {
            Cell.Tile = null;
        }

        Cell = cell;
        Cell.Tile = this;

        transform.position = Cell.transform.position;
    }

    public void MoveTo(TileCell cell)
    {
        if (Cell != null)
        {
            Cell.Tile = null;
        }

        Cell = cell;
        Cell.Tile = this;

        StartCoroutine(Animate(cell.transform.position, false));
        _audioSource.PlayOneShot(_moveClip);
    }

    public void Merge(TileCell mergoTo)
    {
        if (Cell != null)
        {
            Cell.Tile = null;
        }

        Cell = null;
        mergoTo.Tile.IsLocked = true; // Disable merging to this tile in the current movement

        StartCoroutine(Animate(mergoTo.transform.position, true));
        _audioSource.PlayOneShot(_mergeClip);
    }


    private IEnumerator Animate(Vector3 destPosition, bool isMerging) 
    {
        float elapsed = 0f;
        float duration = 0.1f;

        Vector3 from = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(from, destPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = destPosition;

        if (isMerging)
        {
            Destroy(gameObject);
        }
    }
}
