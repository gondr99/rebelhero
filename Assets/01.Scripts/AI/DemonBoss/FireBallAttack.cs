using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallAttack : DemonBossAttack
{
    [SerializeField]
    private int _fireBallCount = 9;
    private float _fireTerm = 0.4f, _distance = 1f;
    private Transform _firePos;

    [SerializeField]
    private BulletDataSO _bulletData;

    protected override void Awake()
    {
        base.Awake();
        _firePos = transform.parent.Find("FireballPosition");
    }

    public override void Attack(Action Callback)
    {
        StartCoroutine(FireSequence(Callback));
    }

    IEnumerator FireSequence(Action Callback)
    {
        WaitForSeconds ws = new WaitForSeconds(_fireTerm);
        Vector3 offsetPosition = new Vector3( - _fireBallCount / 2f * _distance, 0);
        Vector3 startPos = _firePos.position + offsetPosition; 

        for (int i = 0; i < _fireBallCount; i++)
        {
            yield return ws;
            Vector3 currentPosition = startPos + new Vector3(i * _distance, 0);
            Vector2 aimDirection = _aiBrain.target.position - currentPosition;
            float desireAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

            Quaternion rot = Quaternion.AngleAxis(desireAngle, Vector3.forward); //z축 기준 회전
            Bullet bullet = PoolManager.Instance.Pop(_bulletData.prefab.name) as Bullet;
            bullet.SetPositionAndRotation(currentPosition, rot);
            bullet.IsEnemy = true;
            bullet.BulletData = _bulletData;
            bullet.damageFactor = 1;
        }

        Callback?.Invoke();
    }
}
