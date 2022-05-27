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

    protected bool _isDead = false; //�Ѱ��� �Ѿ��� �������� ������ �����ִ� ���� ���� ����.

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

            if (_isEnemy) //���Ѿ��̸� ���� Player�̴�
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

        //�Ǿư� ���ٸ� �浹 �����ϰ� ����
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
        //���⿣ �ǰ� ����Ʈ�� ��Ÿ�� ����
        IKnockBack kb = collision.GetComponent<IKnockBack>();

        kb?.KnockBack(transform.right, _bulletData.knockBackPower, _bulletData.knockBackDelay);

        Vector2 randomOffset = Random.insideUnitCircle * 0.5f; // ���� 1¥�� ������ ������ǥ �ϳ� ã�Ƽ� 1/2��
        ImpactScript impact = PoolManager.Instance.Pop(_bulletData.impactEnemyPrefab.name) as ImpactScript;
        Quaternion rot = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360)));
        impact.SetPositionAndRotation(collision.transform.position + (Vector3)randomOffset, rot);
    }

    //��ֹ��� �ε����� ��
    private void HitObstacle(Collider2D collision)
    {
        //��ֹ��� �ʻ� ��¥�⶧���� ��ġ�� �������� �߽���ġ�� ����������.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 10f);
        //RaycastHit2D�� ����ü��. ���� �ȿ� �ִ� collider�� �˻��ؾ���.
        if (hit.collider != null)
        {
            ImpactScript impact = PoolManager.Instance.Pop(_bulletData.impactObstaclePrefab.name) as ImpactScript;
            Quaternion rot = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360)));
            impact.SetPositionAndRotation(hit.point + (Vector2)transform.right * 0.5f, rot);
            //�ణ ������ �����ؼ� ���� ���� ���� �Ͱ��� ����Ʈ ����
        }
    }

    public override void Reset()
    {
        damageFactor = 1;
        _timeToLive = 0;
        _isDead = false;
    }
}
