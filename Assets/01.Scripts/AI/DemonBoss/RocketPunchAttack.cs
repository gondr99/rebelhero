using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketPunchAttack : DemonBossAttack
{
    private Transform _leftHand;
    private Transform _rightHand;

    protected override void Awake()
    {
        base.Awake();
        _leftHand = transform.parent.Find("LeftHand");
        _rightHand = transform.parent.Find("RightHand");
    }

    public override void Attack(Action Callback)
    {
        
    }
}
