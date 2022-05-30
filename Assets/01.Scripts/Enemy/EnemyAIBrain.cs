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

    public Transform basePosition = null;

    private void Awake()
    {
        _aiActionData = transform.Find("AI").GetComponent<AIActionData>();
    }
    public void SetAttackState(bool state)
    {
        _aiActionData.attack = state;
    }

    private void Start()
    {
        target = GameManager.Instance.PlayerTrm;
    }

    private void Update()
    {
        //Ÿ���� ������ ������ ����
        if (target == null)
        {
            OnMovementKeyPress?.Invoke(Vector2.zero);
        }
        else
        {
            currentState.UpdateState(); //���� ���¸� ������Ʈ
        }

    }

    public void Attack()
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