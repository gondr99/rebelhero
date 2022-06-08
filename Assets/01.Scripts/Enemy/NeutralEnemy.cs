using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NeutralEnemy : Enemy
{
    protected bool _isNeutral = false;  //무력 상태가 되면 true
    protected int _accumulateDamage = 0; //누적데미지

    [SerializeField]
    protected int _neutralDamage = 10;
    [SerializeField]
    protected float _neutralTime = 3f; //무력화 시간
    public UnityEvent NeutralHit = null; //무력상태일때 맞으면 코인 드랍
    public UnityEvent OnEnterNeutral = null;
    public UnityEvent OnExitNeutral = null;

    public override void GetHit(int damage, GameObject damageDealer)
    {
        //사망했거나 공격중일때는 데미지 안받도록 한다.
        if (_isDead || _enemyBrain.AIActionData.attack) return;

        if(_isNeutral)
        {
            NeutralHit?.Invoke();
        }else
        {
            base.GetHit(damage, damageDealer);
            _accumulateDamage += damage; //매 10의 데미지마다 무력화에 빠지도록
        }
        
        if(_isNeutral == false && _accumulateDamage >= _neutralDamage)
        {
            _accumulateDamage = 0;
            _isNeutral = true;
            OnEnterNeutral?.Invoke();
            StartCoroutine(NeutralCoroutine());
        }
    }

    IEnumerator NeutralCoroutine()
    {
        yield return new WaitForSeconds(_neutralTime);
        _isNeutral = false;
        OnExitNeutral?.Invoke();
    }

    public override void Reset()
    {
        base.Reset();
        _accumulateDamage = 0;
        _isNeutral = false;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public override void PerformAttack()
    {
        //무력상태일때는 공격 안하게
        if (_isNeutral)
            return;
        base.PerformAttack();
    }
}
