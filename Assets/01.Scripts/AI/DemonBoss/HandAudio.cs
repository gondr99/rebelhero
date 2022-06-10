using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAudio : AudioPlayer
{
    [SerializeField] private AudioClip _swingClip;

    public void PlaySwingClip()
    {
        PlayClipWithVariablePitch(_swingClip);
    }
}
