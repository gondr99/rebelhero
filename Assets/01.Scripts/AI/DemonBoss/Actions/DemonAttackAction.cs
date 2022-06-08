using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DemonAttackAction : DemonBossAIAction
{
    [SerializeField]
    private DemonBossAIBrain.AttackType _attackType;
    public override void TakeAction()
    {
        //요기 고쳐야해.
        FieldInfo fInfo = typeof(AIDemonBossPhaseData).GetField(_attackType.ToString(), BindingFlags.Public | BindingFlags.Instance);
        bool check = (bool)fInfo.GetValue(_phaseData);
        if(check == false && _phaseData.idleTime <= 0)
        {
            _demonBrain.Attack(_attackType);
        }
    }
}
