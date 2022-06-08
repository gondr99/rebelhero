using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIAction : MonoBehaviour
{
    protected AIActionData _aIActionData;
    protected AIMovementData _aIMovementData;
    [SerializeField]
    protected EnemyAIBrain _enemyBrain;

    protected virtual void Awake()
    {
        _enemyBrain = transform.GetComponentInParent<EnemyAIBrain>();
        _aIActionData = _enemyBrain.transform.GetComponentInChildren<AIActionData>();
        _aIMovementData = _enemyBrain.transform.GetComponentInChildren<AIMovementData>();
    }

    public abstract void TakeAction();
}
