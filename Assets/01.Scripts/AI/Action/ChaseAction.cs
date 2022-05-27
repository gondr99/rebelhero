using UnityEngine;

public class ChaseAction : AIAction
{
    public override void TakeAction()
    {
        if(_aIActionData.attack == true)
        {
            _aIActionData.attack = false;
        }
        Vector2 direction = _enemyBrain.target.transform.position - transform.position;
        //����� ���ɻ縦 �����Ѵ�.
        _aIMovementData.direction = direction.normalized;
        _aIMovementData.pointOfInterest = _enemyBrain.target.transform.position;

        _enemyBrain.Move(_aIMovementData.direction, _aIMovementData.pointOfInterest);
    }
}
