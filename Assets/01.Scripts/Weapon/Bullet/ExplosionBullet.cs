using UnityEngine;

public class ExplosionBullet : RegularBullet
{
    //RegularBullet�� private �� protected�� �����Ұ�
    protected Collider2D _collider;
    
    private bool _charging = false; //��¡ �ҷ�����

    public override BulletDataSO BulletData
    {
        get => _bulletData;
        set
        {
            base.BulletData = value;
            
            if (_collider == null)
            {
                _collider = GetComponent<Collider2D>();
            }
            _charging = _bulletData.isCharging;
            _collider.enabled = !_charging; //��¡�� �ƴ� �Ѿ��� �ٷ� Ȱ��ȭ 
        }  
    }


    //��¡������ �߻� ���� (�ִϸ����� Ʈ���ŷ� ó���ϴ��� �ƴϸ� Ű���� �Է����� ó���ϴ���)
    public void StartFire()
    {
        _collider.enabled = true; //��¡�Ϸ�ú��� �ö��̴� �����
        _charging = false; //��¡�� �Ϸ�Ǿ����� �˷��� �߻�ǵ��� ��.
    }

    //�θ��� fixedUpdate�� virtual�� ����
    protected override void FixedUpdate()
    {
        if (_charging) return; //��¡���϶��� �߻� �ȵ�.

        _timeToLive += Time.fixedDeltaTime;

        if (_rigidbody2d != null && BulletData != null)
        {
            _rigidbody2d.MovePosition(transform.position + BulletData.bulletSpeed * transform.right * Time.fixedDeltaTime);
        }

        if (_timeToLive >= _bulletData.lifeTime)
        {
            _isDead = true;
            PoolManager.Instance.Push(this);
        }
    }

    public override void Reset()
    {
        base.Reset();
        if (_collider != null)
            _collider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isDead) return;

        IHittable hittable = collision.GetComponent<IHittable>();

        //�Ǿư� ���ٸ� �浹 �����ϰ� ����
        if (hittable != null && hittable.IsEnemy == IsEnemy) return;


        //�Ǿư� ���� ���� �浹�� �����ߴٸ� �ϴ� ���� ����

        

        //�̰� ���� �ݰ� �����ؼ� ������
        Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
        ImpactScript impact = PoolManager.Instance.Pop(_bulletData.impactEnemyPrefab.name) as ImpactScript;
        Quaternion rot = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360)));
        impact.SetPositionAndRotation(transform.position + (Vector3)randomOffset, rot);

        //���߹ݰ泻�� ���� �ִ��� Ȯ���ؼ� ���߽�Ŵ
        LayerMask enemyLayerMask = 1 << _enemyLayer;
        Collider2D[] enemyArr = Physics2D.OverlapCircleAll(transform.position, _bulletData.explosionRadius, enemyLayerMask);
        foreach (Collider2D enemy in enemyArr)
        {
            IHittable hit = enemy.GetComponent<IHittable>();
            hit?.GetHit(_bulletData.damage * damageFactor, gameObject);

            IKnockBack kb = enemy.GetComponent<IKnockBack>();
            Vector3 kbDir = (enemy.transform.position - transform.position).normalized;
            kb?.KnockBack(kbDir, _bulletData.knockBackPower, _bulletData.knockBackDelay);
        }

        _isDead = true;
        PoolManager.Instance.Push(this);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.Selection.activeObject == gameObject && _bulletData != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _bulletData.explosionRadius);
            Gizmos.color = Color.white;
        }
    }
#endif
}