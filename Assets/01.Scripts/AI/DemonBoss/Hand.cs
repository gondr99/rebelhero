using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Hand : MonoBehaviour, IHittable
{
    private Animator _animator;
    private readonly int _hashFadeIn = Animator.StringToHash("FadeIn");
    private readonly int _hashShockAttack = Animator.StringToHash("ShockAttack");
    private readonly int _hashFlapperAttack = Animator.StringToHash("FlapperAttack");

    private Vector3 _initPosition;

    public bool IsEnemy => true;

    public Vector3 _hitPoint { get; set; }

    #region �ǰ�ó�� ����
    private int _hp;
    public int HP => _hp;
    [SerializeField]
    private bool _isDead;
    
    public UnityEvent OnGetHit = null; //�ǰݽ� �߻��� ����Ʈ
    public UnityEvent OnDead = null; //�ش� ���� �ı��Ǿ��� �� �߻��� ����Ʈ
    #endregion

    #region ���ݰ���
    Sequence seq = null;
    public UnityEvent OnAttackFeedback = null;
    public UnityEvent OnFlapFeedback = null;

    [SerializeField] private Transform _attackPosTrm;
    [SerializeField] private float _atkRadius = 3f;
    [SerializeField] private int _damage;
    [SerializeField] private float _knockPower, _flapPower, _flapDistance;
    [SerializeField] private bool _isLeftHand;
    private bool _isFlapping; //�ĸ�ä �������϶��� FixedUpdate���� �浹�˻�
    private LayerMask _whatIsEnemy;
    #endregion

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _attackPosTrm = transform.Find("AttackPoint");
        _initPosition = transform.position; //�ʱ� ������ �����صΰ�
        _whatIsEnemy = 1 << LayerMask.NameToLayer("Player");
    }

    private void OnDisable()
    {
        seq?.Kill();
        SetDeadParam(); 
    }

    public void InitHand(int maxHp)
    {
        _hp = maxHp;
    }

    #region ��ũ���̺� ����
    public void AttackShockSequence(Vector3 targetPos, Action Callback)
    {
        Vector3 atkPos = targetPos - _attackPosTrm.localPosition; //���ð��� ���ָ� ��Ÿ��ŭ������.
        seq = DOTween.Sequence();
        //���⿡ ���������� �߰��ؾ��Ѵ�.
        seq.Append(transform.DOMove(_initPosition + new Vector3(0, 0.5f), 0.2f));
        seq.Join(transform.DOScale(1.2f, 0.2f));
        seq.Append(transform.DOMove(atkPos + new Vector3(0, 0.5f), 0.7f));
        seq.AppendCallback(() =>
        {
            _animator.SetTrigger(_hashShockAttack);
        });
        seq.Join(transform.DOMove(atkPos, 0.2f));
        seq.AppendCallback(() =>
        {
            
            ActiveShock();
        });
        seq.AppendInterval(1f);
        seq.Append(transform.DOMove(_initPosition, 0.3f));
        seq.AppendCallback(() => Callback?.Invoke());
    }

    private void ActiveShock()
    {
        OnAttackFeedback?.Invoke(); //ī�޶� ��鸲 ���� �ǵ��
        //��ũ ����Ʈ ����ϰ�
        ImpactScript impact = PoolManager.Instance.Pop("ImpactShockwave") as ImpactScript;
        impact.SetPositionAndRotation(_attackPosTrm.position, Quaternion.identity);
        impact.SetLocalScale(Vector3.one * 1.4f);

        Collider2D col = Physics2D.OverlapCircle(_attackPosTrm.position, _atkRadius, _whatIsEnemy);

        if(col != null)
        {
            IHittable iHit = col.GetComponent<IHittable>();
            iHit.GetHit(_damage, gameObject);

            Vector3 dir = col.transform.position - _attackPosTrm.position;
            IKnockBack iKnock = col.GetComponent<IKnockBack>();
            if (dir.sqrMagnitude == 0)  //����� ������ ������������ Ƣ����
            {
                dir = Random.insideUnitCircle;
            }

            iKnock?.KnockBack(dir.normalized, _knockPower, 1f);
        }

    }
    #endregion

    #region �ĸ�ä ����
    public void AttackFlapperSequence(Vector3 targetPos, Action Callback)
    {
        Vector3 atkPos = targetPos - _attackPosTrm.localPosition; //���ð��� ���ָ� ��Ÿ��ŭ������.

        seq = DOTween.Sequence();
        seq.Append(transform.DOMoveY(atkPos.y, 0.4f)); //0.4�ʰ� �ָ� ������ ����
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() =>
        {
            _animator.SetTrigger(_hashFlapperAttack); //�չٴ� �ִϸ��̼� ���
            _isFlapping = true; //�չٴ� ���� ��!
            OnFlapFeedback?.Invoke(); //���� �� �ǵ�� ���
        });
        float x = _isLeftHand  ? -1f : 1f;
        float targetX = transform.position.x + x * _flapDistance;
        seq.Join(transform.DOMoveX(targetX, 0.7f));
        seq.AppendInterval(0.3f);
        seq.AppendCallback(() =>
        {
            _isFlapping = false; //�չٴ� ���� ����
        });
        seq.Join(transform.DOMove(_initPosition, 0.3f));
        seq.AppendCallback(() => Callback?.Invoke());
    }

    private void FixedUpdate()
    {
        //�ĸ�ä ����϶��� �������� ���ϵ���
        if(_isFlapping)
        {
            Vector3 pos = _attackPosTrm.position;

            //���� 1 ���� 2����¥�� �簢������ �˻�
            Collider2D col = Physics2D.OverlapArea(pos - new Vector3(0.5f, 1f), pos + new Vector3(0.5f, 1f), _whatIsEnemy);

            if(col != null)
            {
                _isFlapping = false; //�Ѵ븸 �������� ���ְ�

                IHittable iHit = col.GetComponent<IHittable>();
                iHit.GetHit(_damage, gameObject);

                Vector3 dir = _isLeftHand ? new Vector3(-1, -1) : new Vector3(1, -1);
                IKnockBack iKnock = col.GetComponent<IKnockBack>();

                iKnock?.KnockBack(dir.normalized, _flapPower, 1f);
            }
        }
    }

    #endregion

    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.P))
    //    {
    //        AttackShockSequence(Vector3.zero);
    //    }
    //    if (Input.GetKeyDown(KeyCode.O))
    //    {
    //        AttackFlapperSequence(Vector3.zero);
    //    }
    //}


    public void GetHit(int damage, GameObject damageDealer)
    {
        if (_isDead) return;

        OnGetHit?.Invoke();
        _hp -= damage;

        _hitPoint = transform.position + (Vector3)Random.insideUnitCircle; //�ǰ����� ����

        if (_hp <= 0)
        {
            seq?.Kill();  //����� ����Ǵ� �������� ų
            seq = null;

            _hp = 0;
            _isDead = true;
            OnDead?.Invoke();
        }
    }

    public void SetDeadParam()
    {
        gameObject.SetActive(false);
        transform.SetPositionAndRotation(_initPosition, Quaternion.identity);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.Selection.activeObject == gameObject)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_attackPosTrm.position, _atkRadius);
            Gizmos.color = Color.white;
        }
    }
#endif   
}