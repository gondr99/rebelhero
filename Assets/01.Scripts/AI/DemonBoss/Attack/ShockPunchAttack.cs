using System;
using System.Collections;
using UnityEngine;

public class ShockPunchAttack : DemonBossAttack
{
    private DemonBossAIBrain _brain;

    protected override void Awake()
    {
        base.Awake();
        _brain = transform.parent.GetComponent<DemonBossAIBrain>();
    }

    public override void Attack(Action Callback)
    {
        StartCoroutine(PunchSequence(Callback));
    }

    IEnumerator PunchSequence(Action Callback)
    {

        if(_brain.LeftHand.gameObject.activeSelf == false && _brain.RightHand.gameObject.activeSelf == false)
        {
            Callback?.Invoke();
            yield break;
        }    

        Hand first = _brain.LeftHand;
        Hand second = null;
        if (first.gameObject.activeSelf == false)
        {
            first = _brain.RightHand;
        }
        else if(_brain.RightHand.gameObject.activeSelf == true)
        {
            second = _brain.RightHand;
        }

        first.AttackShockSequence(_brain.target.position, null);
        yield return new WaitForSeconds(1f);
        if (second != null)
        {
            second.AttackShockSequence(_brain.target.position, () => Callback?.Invoke());
        }else
        {
            Callback?.Invoke();
        }
    }
}
