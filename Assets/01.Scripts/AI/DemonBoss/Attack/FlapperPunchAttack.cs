using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlapperPunchAttack : DemonBossAttack
{
    private DemonBossAIBrain _brain;

    protected override void Awake()
    {
        base.Awake();
        _brain = transform.parent.GetComponent<DemonBossAIBrain>();
    }

    public override void Attack(Action<bool> Callback)
    {
        StartCoroutine(FlapperSequence(Callback));
    }

    IEnumerator FlapperSequence(Action<bool> Callback)
    {

        if (_brain.LeftHand.gameObject.activeSelf == false && _brain.RightHand.gameObject.activeSelf == false)
        {
            Callback?.Invoke(false);
            yield break;
        }

        if (_brain.LeftHand.gameObject.activeSelf == false && _brain.RightHand.gameObject.activeSelf == false)
        {
            Callback?.Invoke(false);
            yield break;
        }

        if (_brain.LeftHand.gameObject.activeSelf == false)
        {
            _brain.RightHand.AttackFlapperSequence(_brain.target.position, () => Callback?.Invoke(true));
        }
        else
        {
            _brain.LeftHand.AttackFlapperSequence(_brain.target.position, null);
            yield return new WaitForSeconds(1f);
            if (_brain.RightHand.gameObject.activeSelf == false)
            {
                Callback?.Invoke(false);
            }
            else
            {
                _brain.RightHand.AttackFlapperSequence(_brain.target.position, () => Callback?.Invoke(true));
            }
        }

    }
}
