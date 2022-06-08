using UnityEngine;

public abstract class AIDecision : MonoBehaviour
{
    protected AIActionData _aIActionData;
    protected AIMovementData _aIMovementData;
    protected EnemyAIBrain _enemyBrain;

    protected virtual void Awake()
    {
        _enemyBrain = transform.GetComponentInParent<EnemyAIBrain>();
        _aIActionData = _enemyBrain.transform.Find("AI").GetComponent<AIActionData>();
        _aIMovementData = _enemyBrain.transform.Find("AI").GetComponent<AIMovementData>();

    }

    public abstract bool MakeADecision();
}
