using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//다음공격이 지정한 공격과 타입이 일치하는지 체크
public class CheckNextAttackTypeDecision : DemonBossDecison
{
    [SerializeField]
    private DemonBossAIBrain.AttackType attackType;

    public override bool MakeADecision()
    {
        return _phaseData.nextAttackType == attackType;
    }
}
