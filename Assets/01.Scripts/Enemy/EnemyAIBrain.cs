using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAIBrain : MonoBehaviour, IAgentInput
{ 
    public Transform target;

    [field: SerializeField] public UnityEvent<Vector2> OnMovementKeyPress { get; set; }
    [field: SerializeField] public UnityEvent<Vector2> OnPointerPositionChanged { get; set; }
    [field: SerializeField] public UnityEvent OnFireButtonPress { get; set; }
    [field: SerializeField] public UnityEvent OnFireButtonRelease { get; set; }

    public AIState currentState;
    private AIActionData _aiActionData;
    public AIActionData AIActionData => _aiActionData;

    public Transform basePosition = null;

    protected virtual void Awake()
    {
        _aiActionData = transform.Find("AI").GetComponent<AIActionData>();
    }
    public void SetAttackState(bool state)
    {
        _aiActionData.attack = state;
    }

    protected void Start()
    {
        target = GameManager.Instance.PlayerTrm; //플레이어를 타겟으로 잡아 넣고
    }

    protected virtual void Update()
    {
        //타겟이 없으면 적군은 정지
        if (target == null)
        {
            OnMovementKeyPress?.Invoke(Vector2.zero);
        }
        else
        {
            currentState.UpdateState(); //현재 상태를 업데이트
        }

    }

    public virtual void Attack()
    {
        OnFireButtonPress?.Invoke();
    }

    public void Move(Vector2 moveDirection, Vector2 targetPosition)
    {
        OnMovementKeyPress?.Invoke(moveDirection);
        OnPointerPositionChanged?.Invoke(targetPosition);
    }

    public void ChangeToState(AIState nextState)
    {
        currentState = nextState;
    }
}
