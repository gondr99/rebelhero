using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//idleTime이 0보다 작거나 같아야 true
public class DelayTimeDecision : DemonBossDecison
{
    public override bool MakeADecision()
    {
        return _phaseData.idleTime <= 0;  
    }
}
