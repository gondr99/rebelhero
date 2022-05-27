using UnityEngine;

public class IdleAction : AIAction
{
    public override void TakeAction()
    {
        _aIMovementData.direction = Vector2.zero;
        _aIMovementData.pointOfInterest = transform.position; //�ڱ��ڽſ��� ����
        _enemyBrain.Move(_aIMovementData.direction, _aIMovementData.pointOfInterest);
    }
}
