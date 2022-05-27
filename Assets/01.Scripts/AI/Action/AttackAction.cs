using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : AIAction
{
    public override void TakeAction()
    {
        _aIMovementData.direction = Vector2.zero;
        _aIMovementData.pointOfInterest = _enemyBrain.target.transform.position;
        _enemyBrain.Move(_aIMovementData.direction, _aIMovementData.pointOfInterest);
        //_aIActionData.attack = true;
        _enemyBrain.Attack();
    }
}
