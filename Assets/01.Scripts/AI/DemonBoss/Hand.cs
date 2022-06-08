using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    private Animator _animator;
    private readonly int _hashFadeIn = Animator.StringToHash("FadeIn");
    private readonly int _hashShockAttack = Animator.StringToHash("ShockAttack");
    private readonly int _hashFlapperAttack = Animator.StringToHash("FlapperAttack");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
}
