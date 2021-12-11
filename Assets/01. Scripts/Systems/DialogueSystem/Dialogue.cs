using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HollowKnight {

    [System.Serializable]
    public class Dialogue
    {
        public string[] names;
        public Sprite[] avatars;
        public Sprite[] textboxs;
        public UnityEvent[] callbacks;

        [TextArea(5, 10)]
        public string[] sentences;
    }
}

