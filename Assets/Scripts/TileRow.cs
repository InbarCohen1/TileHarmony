using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRow : MonoBehaviour
{
    public TileCell[] _cells { get; private set; }

    private void Awake()
    {
        
        _cells =  GetComponentsInChildren<TileCell>();
    }
}
