using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DemonBossAttack : MonoBehaviour
{
    protected DemonBossAIBrain _aiBrain;

    protected virtual void Awake()
    {
        _aiBrain = transform.parent.GetComponent<DemonBossAIBrain>();
    }

    public abstract void Attack(Action Callback);
}
