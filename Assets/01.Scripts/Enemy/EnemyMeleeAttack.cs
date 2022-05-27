using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : EnemyAttack
{
    public override void Attack(int damage)
    {
        if (_waitBeforeNextAttack == false)
        {
            _enemyBrain.SetAttackState(true);
            AttackFeedback?.Invoke();
            IHittable hittable = GetTarget().GetComponent<IHittable>();
            AttackFeedback?.Invoke();
            hittable?.GetHit(damage, gameObject);
            StartCoroutine(WaitBeforeAttackCoroutine());

        }
    }

}
