using UnityEngine;

public abstract class AIDecision : MonoBehaviour
{
    protected AIActionData _aIActionData;
    protected AIMovementData _aIMovementData;
    protected EnemyAIBrain _enemyBrain;

    private void Awake()
    {
        _enemyBrain = transform.GetComponentInParent<EnemyAIBrain>();
        _aIActionData = _enemyBrain.transform.GetComponentInChildren<AIActionData>();
        _aIMovementData = _enemyBrain.transform.GetComponentInChildren<AIMovementData>();


        ChildAwake();
    }

    protected virtual void ChildAwake()
    {
        //�ڽ� Awake���� ���ٰ� ������ ���⼭ ����
    }

    public abstract bool MakeADecision();
}
