using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Enemy : PoolableMono, IHittable, IAgent, IKnockBack
{
    [SerializeField]
    protected EnemyDataSO _enemyData;
    public EnemyDataSO EnemyData
    {
        get => _enemyData;
    }
    public int Health { get; private set; }
    public Vector3 _hitPoint { get; private set; }

    protected EnemyAttack _enemyAttack;
    protected bool _isDead = false;

    //���� �˹� ó���� ���� ������Ʈ �����Ʈ ��������
    protected AgentMovement _agentMovement;
    protected EnemyAIBrain _enemyBrain;

    [field: SerializeField] public UnityEvent OnGetHit { get; set; }
    [field: SerializeField] public UnityEvent OnDie { get; set; }
    [field: SerializeField] public UnityEvent OnReset { get; set; }
    

    public bool IsEnemy => true;

    [SerializeField]
    protected bool _isActive = false;

    protected virtual void Awake()
    {
        _agentMovement = GetComponent<AgentMovement>();
        _enemyBrain = GetComponent<EnemyAIBrain>();

        _enemyAttack = GetComponent<EnemyAttack>();
        _enemyAttack.attackDelay = _enemyData.attackDelay;
    }

    protected void Start()
    {
        Health = _enemyData.maxHealth;
    }

    public virtual void PerformAttack()
    {
        if (!_isDead && _isActive)
        {
            //���⿡ ���ݾִϸ��̼ǰ� �������� ������ ����.
            _enemyAttack.Attack(_enemyData.damage);
        }
    }

    public virtual void GetHit(int damage, GameObject damageDealer)
    {
        if (_isDead) return;

        //ġ��Ÿ���� ���� ������ ���
        
        bool isCritical = GameManager.Instance.IsCritical;
        if(isCritical)
        {
            damage = GameManager.Instance.GetCriticalDamage(damage);
        }

        Health -= damage;
        _hitPoint = damageDealer.transform.position;
        //�ǰݽ�Ų �༮�� ������ ���� ����Ʈ ����� �ʿ�
        OnGetHit?.Invoke(); //�ǰݿ� ���� �ǵ��� ���

        //������ ���� �������
        PopupText damagePopup = PoolManager.Instance.Pop("PopupText") as PopupText;
        damagePopup?.Setup(damage, transform.position + new Vector3(0, 0.5f, 0), isCritical, Color.white);

        //��� ó��
        if(Health <= 0)
        {
            DeadProcess();
        }
    }

    public void DeadProcess()
    {
        if (_isDead) return; //�̹� ����ó���� �ǰ� �ִ� �ֵ��� �ȹ޵���
        Health = 0;
        _isDead = true;
        _agentMovement.StopImmediatelly();
        _agentMovement.enabled = false;
        OnDie?.Invoke();//�׾��� �� �ǵ���� �����Ѵ�.
    }

    public override void Reset()
    {
        OnReset?.Invoke();
        Health = _enemyData.maxHealth;
        _isActive = false;
        _isDead = false;

        _agentMovement.enabled = true;
        _enemyAttack.Reset(); //ó���������� �� ���� ��Ÿ�� ���ư���
        _agentMovement.ResetKnockBackParam();
        
    }

    public void Die()
    {
        PoolManager.Instance.Push(this);
    }

    //��Ż���� ������ �� ����
    public void SpawnInPortal(Vector3 pos, float power, float time)
    {
        _isActive = false;
        transform.DOJump(pos, power, 1, time).OnComplete(()=> { _isActive = true; });
        
    }

    public void KnockBack(Vector2 direction, float power, float duration)
    {
        if (!_isDead && _isActive)
        {
            if (power > _enemyData.knockRegist)
                _agentMovement.KnockBack(direction, power - _enemyData.knockRegist, duration);
        }
    }
}
