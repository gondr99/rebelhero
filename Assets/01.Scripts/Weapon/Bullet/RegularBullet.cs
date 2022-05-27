using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class RegularBullet : Bullet
{
    protected Rigidbody2D _rigidbody2d;
    protected SpriteRenderer _spriteRenderer;
    protected Animator _animator;
    [SerializeField]
    protected float _timeToLive;

    protected int _enemyLayer;
    protected int _obstacleLayer;

    protected bool _isDead = false; //한개의 총알이 여러명의 적에게 영향주는 것을 막기 위함.

    public override BulletDataSO BulletData
    {
        get => _bulletData;
        set
        {
            
            _bulletData = value;
            if (_rigidbody2d == null)
            {
                _rigidbody2d = GetComponent<Rigidbody2D>();
            }
            _rigidbody2d.drag = _bulletData.friction;
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }
            _spriteRenderer.sprite = _bulletData.sprite;
            _spriteRenderer.material = _bulletData.bulletMaterial;
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
            _animator.runtimeAnimatorController = _bulletData.animatorController;

            if (_isEnemy) //적총알이면 적은 Player이다
                _enemyLayer = LayerMask.NameToLayer("Player");
            else
                _enemyLayer = LayerMask.NameToLayer("Enemy");
        }
    }

    private void Awake()
    {
        _obstacleLayer = LayerMask.NameToLayer("Obstacle");
    }

    protected virtual void FixedUpdate()
    {
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isDead) return;

        IHittable hittable = collision.GetComponent<IHittable>();

        //피아가 같다면 충돌 무시하고 진행
        if (hittable != null && hittable.IsEnemy == IsEnemy)
        {
            return;
        }
        hittable?.GetHit(_bulletData.damage * damageFactor, gameObject);

        if (collision.gameObject.layer == _obstacleLayer)
        {
            HitObstacle(collision);
        }

        if (collision.gameObject.layer == _enemyLayer)
        {
            HitEnemy(collision);
        }
        _isDead = true;
        PoolManager.Instance.Push(this);
    }

    private void HitEnemy(Collider2D collision)
    {
        //여기엔 피격 임팩트가 나타날 예정
        IKnockBack kb = collision.GetComponent<IKnockBack>();

        kb?.KnockBack(transform.right, _bulletData.knockBackPower, _bulletData.knockBackDelay);

        Vector2 randomOffset = Random.insideUnitCircle * 0.5f; // 길이 1짜리 원상의 랜덤좌표 하나 찾아서 1/2로
        ImpactScript impact = PoolManager.Instance.Pop(_bulletData.impactEnemyPrefab.name) as ImpactScript;
        Quaternion rot = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360)));
        impact.SetPositionAndRotation(collision.transform.position + (Vector3)randomOffset, rot);
    }

    //장애물에 부딛혔을 때
    private void HitObstacle(Collider2D collision)
    {
        //장애물은 맵상에 통짜기때문에 위치를 가져오면 중심위치가 가져와진다.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 10f);
        //RaycastHit2D는 구조체야. 따라서 안에 있는 collider를 검사해야해.
        if (hit.collider != null)
        {
            ImpactScript impact = PoolManager.Instance.Pop(_bulletData.impactObstaclePrefab.name) as ImpactScript;
            Quaternion rot = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360)));
            impact.SetPositionAndRotation(hit.point + (Vector2)transform.right * 0.5f, rot);
            //약간 앞으로 생성해서 실제 벽에 맞은 것같은 이펙트 생성
        }
    }

    public override void Reset()
    {
        damageFactor = 1;
        _timeToLive = 0;
        _isDead = false;
    }
}
