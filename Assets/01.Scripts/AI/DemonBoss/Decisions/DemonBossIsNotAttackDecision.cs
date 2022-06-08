using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonBossIsNotAttackDecision : DemonBossDecison
{
    public override bool MakeADecision()
    {
        //모두 공격을 하고 있지 않다면 
        return _phaseData.CanAttack;
    }
}
