using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//액티브상태인 보스만 공격으로 이동하도록
public class IsActiveDecision : DemonBossDecison
{
    public override bool MakeADecision()
    {
        return _phaseData.isActive;
    }
}
