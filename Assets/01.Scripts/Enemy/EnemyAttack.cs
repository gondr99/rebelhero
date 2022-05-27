using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EnemyAttack : MonoBehaviour
{
    protected EnemyAIBrain _enemyBrain;
    protected Enemy _enemy;

    public UnityEvent AttackFeedback;

    [field: SerializeField]
    public float attackDelay { get; set; } = 1;

    protected bool _waitBeforeNextAttack;

    public bool WaitingForNextAttack
    {
        get => _waitBeforeNextAttack;
    }
    //[SerializeField]
    //protected bool _isAttacking = false;
    ////���� ������������ üũ�ϴ� ���� ���ݾִϸ��̼� ���� �� �ȿ����� ��鶧���� �ʿ���.

    //public bool IsAttacking
    //{
    //    get => _isAttacking;
    //}

    private void Awake()
    {
        _enemyBrain = GetComponent<EnemyAIBrain>();
        _enemy = GetComponent<Enemy>();
        AwakeChild();
    }

    public virtual void AwakeChild()
    {
        //do nothing here;
    }

    public abstract void Attack(int damage);

    protected IEnumerator WaitBeforeAttackCoroutine()
    {
        _waitBeforeNextAttack = true;
        yield return new WaitForSeconds(attackDelay);
        _waitBeforeNextAttack = false;
    }

    protected Transform GetTarget()
    {
        return _enemyBrain.target;
    }

    public void Reset()
    {
        StartCoroutine(WaitBeforeAttackCoroutine());
    }

    //�ǰݸ�� �÷������϶� �ؾ��� ��
    public virtual void HitMotionPlay()
    {
        //do nothing here!
    }
}
