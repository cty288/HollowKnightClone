using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManger_Alt : MonoBehaviour
{
    public static AudioSource audioSource;
    public HollowKnight.AudioAssets audioAssets;
    private void Start()
    {
        audioSource.PlayOneShot(audioAssets.backgroundMusic, 0.3f);
    }
}
