using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonBossBodyAnimation : MonoBehaviour
{
    protected Animator _bodyAnimator;

    protected readonly int _hashSummonPortal = Animator.StringToHash("SummonPortal");
    protected readonly int _hashFireBall = Animator.StringToHash("Fireball");

    private void Awake()
    {
        _bodyAnimator = GetComponent<Animator>();
    }

    public void PlaySummonPortalAnimation()
    {
        _bodyAnimator.SetTrigger(_hashSummonPortal);
    }

    public void PlayFireBallAnimation()
    {
        _bodyAnimator.SetTrigger(_hashFireBall);
    }
}
