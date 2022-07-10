using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlapperPunchAttack : DemonBossAttack
{
    

    private bool _complete = false;
   

    public override void Attack(Action<bool> Callback)
    {
        _complete = false; 
        StartCoroutine(FlapperSequence(Callback));
    }

    IEnumerator FlapperSequence(Action<bool> Callback)
    {

        if (_aiBrain.LeftHand.gameObject.activeSelf == false && _aiBrain.RightHand.gameObject.activeSelf == false)
        {
            Callback?.Invoke(false);
            yield break;
        }

        if (_aiBrain.LeftHand.gameObject.activeSelf == false)
        {
            _aiBrain.RightHand.AttackFlapperSequence(_aiBrain.target.position, () =>
            {
                _complete = true;
                Callback?.Invoke(true);
            });
        }
        else
        {
            _aiBrain.LeftHand.AttackFlapperSequence(_aiBrain.target.position, null);
            yield return new WaitForSeconds(1f);
            if (_aiBrain.RightHand.gameObject.activeSelf == false)
            {
                _complete = true;
                Callback?.Invoke(false);
            }
            else
            {
                _aiBrain.RightHand.AttackFlapperSequence(_aiBrain.target.position, () =>
                {
                    _complete = true;
                    Callback?.Invoke(true);
                });
            }
        }

        yield return new WaitForSeconds(2.5f); //2.5초가 지났으면 공격완료가되었어야 한다.
        //안되어 있다면 시퀀스가 중간에 죽어버린거
        if(_complete == false)
        {
            Callback?.Invoke(false);
        }
    }
}
