using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DemonAttackAction : DemonBossAIAction
{
    [SerializeField]
    private DemonBossAIBrain.AttackType _attackType;

    private FieldInfo _fInfo = null;
    protected override void Awake()
    {
        base.Awake();
        _fInfo = typeof(AIDemonBossPhaseData).GetField(_attackType.ToString(), BindingFlags.Public | BindingFlags.Instance);
    }

    public override void TakeAction()
    {        
        bool check = (bool)_fInfo.GetValue(_phaseData);
        if(check == false && _phaseData.idleTime <= 0)
        {
            _demonBrain.Attack(_attackType);
        }
    }
}
