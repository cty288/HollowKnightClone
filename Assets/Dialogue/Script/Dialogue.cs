using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string[] names;
    public Sprite[] avatars;
    public Sprite[] textboxs;

    [TextArea(5,10)]
    public string[] sentences;
}
