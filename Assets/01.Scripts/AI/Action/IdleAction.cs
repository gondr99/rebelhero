using UnityEngine;

public class IdleAction : AIAction
{
    public override void TakeAction()
    {
        _aIMovementData.direction = Vector2.zero;
        _aIMovementData.pointOfInterest = transform.position; //자기자신에게 관심
        _enemyBrain.Move(_aIMovementData.direction, _aIMovementData.pointOfInterest);
    }
}
