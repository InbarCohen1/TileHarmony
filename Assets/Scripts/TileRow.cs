using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRow : MonoBehaviour
{
    public TileCell[] Cells { get; private set; }

    private void Awake()
    {
        Cells =  GetComponentsInChildren<TileCell>();
    }
}
