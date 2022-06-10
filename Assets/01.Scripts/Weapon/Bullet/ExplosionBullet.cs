using UnityEngine;

public class ExplosionBullet : RegularBullet
{
    //RegularBullet의 private 을 protected로 변경할것
    protected Collider2D _collider;
    
    private bool _charging = false; //차징 불렛인지

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
            _collider.enabled = !_charging; //차징이 아닌 총알은 바로 활성화 
        }  
    }


    //차징끝나고 발사 시작 (애니메이터 트리거로 처리하던지 아니면 키보드 입력으로 처리하던지)
    public void StartFire()
    {
        _collider.enabled = true; //차징완료시부터 컬라이더 만들기
        _charging = false; //차징이 완료되었음을 알려서 발사되도록 함.
    }

    //부모의 fixedUpdate를 virtual로 변경
    protected override void FixedUpdate()
    {
        if (_charging) return; //차징중일때는 발사 안됨.

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

        //피아가 같다면 충돌 무시하고 진행
        if (hittable != null && hittable.IsEnemy == IsEnemy) return;


        //피아가 같지 않은 충돌에 도달했다면 일단 폭발 개시

        

        //이건 폭발 반경 조사해서 보내고
        Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
        ImpactScript impact = PoolManager.Instance.Pop(_bulletData.impactEnemyPrefab.name) as ImpactScript;
        Quaternion rot = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360)));
        impact.SetPositionAndRotation(transform.position + (Vector3)randomOffset, rot);

        //폭발반경내에 적이 있는지 확인해서 폭발시킴
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