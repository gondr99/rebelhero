using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : PoolableMono
{
    [SerializeField]
    protected BulletDataSO _bulletData;

    protected bool _isEnemy;

    public bool IsEnemy
    {
        get => _isEnemy;
        set
        {
            _isEnemy = value;
        }
    }
    public int damageFactor = 1; //총알의 데미지 계수. 적의 경우 이 계수를 곱해서 데미지 연산이 이뤄진다.

    public virtual BulletDataSO BulletData
    {
        get => _bulletData;
        set => _bulletData = value;
    }

    public void SetPositionAndRotation(Vector3 pos, Quaternion rot)
    {
        transform.SetPositionAndRotation(pos, rot);
    }
}
