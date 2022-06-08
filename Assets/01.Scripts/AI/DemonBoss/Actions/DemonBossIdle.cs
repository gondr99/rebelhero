using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonBossIdle : DemonBossAIAction
{
    public override void TakeAction()
    {
        //여기서 타이머에 따라서 어떤 액션을 취할지를 결정하면 된다.
        if(_phaseData.idleTime > 0)
        {
            _phaseData.idleTime -= Time.deltaTime;
            if (_phaseData.idleTime < 0)
                _phaseData.idleTime = 0;
        }
            
    }
}
