using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyRangeAttack : EnemyAttack
{
    [SerializeField] private BulletDataSO _bulletData;
    [SerializeField] private Transform _firePos;

    
    public override void Attack(int damage)
    {
        if (_waitBeforeNextAttack == false)
        {
            _enemyBrain.SetAttackState(true);
            AttackFeedback?.Invoke();

            Transform target = GetTarget();

            Vector2 aimDirection = target.position - _firePos.position;

            float desireAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

            Quaternion rot = Quaternion.AngleAxis(desireAngle, Vector3.forward); //z축 기준 회전

            SpawnBullet(_firePos.position, rot, true, damage);

            StartCoroutine(WaitBeforeAttackCoroutine());
        }
    }

    private void SpawnBullet(Vector3 position, Quaternion rot, bool isEnemyBullet, int damage)
    {
        
        Bullet bullet = PoolManager.Instance.Pop(_bulletData.prefab.name) as Bullet;
        bullet.SetPositionAndRotation(position, rot);
        bullet.IsEnemy = isEnemyBullet;
        bullet.BulletData = _bulletData;
        bullet.damageFactor = damage;
    }
}
