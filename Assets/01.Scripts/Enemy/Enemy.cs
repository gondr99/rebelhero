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

    //차후 넉백 처리를 위한 에이전트 무브먼트 가져오기
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
            //여기에 공격애니메이션과 실질적인 공격이 들어간다.
            _enemyAttack.Attack(_enemyData.damage);
        }
    }

    public virtual void GetHit(int damage, GameObject damageDealer)
    {
        if (_isDead) return;

        //치명타율에 따른 데미지 계산
        
        bool isCritical = GameManager.Instance.IsCritical;
        if(isCritical)
        {
            damage = GameManager.Instance.GetCriticalDamage(damage);
        }

        Health -= damage;
        _hitPoint = damageDealer.transform.position;
        //피격시킨 녀석의 포지션 차후 이펙트 재생에 필요
        OnGetHit?.Invoke(); //피격에 대한 피드백들 재생

        //데미지 숫자 띄워주자
        PopupText damagePopup = PoolManager.Instance.Pop("PopupText") as PopupText;
        damagePopup?.Setup(damage, transform.position + new Vector3(0, 0.5f, 0), isCritical, Color.white);

        //사망 처리
        if(Health <= 0)
        {
            DeadProcess();
        }
    }

    public void DeadProcess()
    {
        if (_isDead) return; //이미 죽음처리가 되고 있는 애들은 안받도록
        Health = 0;
        _isDead = true;
        _agentMovement.StopImmediatelly();
        _agentMovement.enabled = false;
        OnDie?.Invoke();//죽었을 때 피드백을 실행한다.
    }

    public override void Reset()
    {
        OnReset?.Invoke();
        Health = _enemyData.maxHealth;
        _isActive = false;
        _isDead = false;

        _agentMovement.enabled = true;
        _enemyAttack.Reset(); //처음시작했을 때 공격 쿨타임 돌아가게
        _agentMovement.ResetKnockBackParam();
        
    }

    public void Die()
    {
        PoolManager.Instance.Push(this);
    }

    //포탈에서 생성된 후 점핑
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
