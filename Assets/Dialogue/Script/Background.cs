using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Background
{
    public GameObject layer;
    Rigidbody2D rb;
    void Awake()
    {
        rb = layer.GetComponent<Rigidbody2D>();
    }
}
