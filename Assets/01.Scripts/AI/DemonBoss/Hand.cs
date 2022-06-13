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

    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCol;

    #region 피격처리 관련
    private int _hp;
    public int HP => _hp;
    [SerializeField]
    private bool _isDead;
    
    public UnityEvent OnGetHit = null; //피격시 발생할 이펙트
    public UnityEvent OnDead = null; //해당 손이 파괴되었을 때 발생할 이펙트
    #endregion

    #region 공격관련
    Sequence seq = null;
    public UnityEvent OnAttackFeedback = null;
    public UnityEvent OnFlapFeedback = null;

    [SerializeField] private Transform _attackPosTrm;
    [SerializeField] private float _atkRadius = 3f;
    [SerializeField] private int _damage;
    [SerializeField] private float _knockPower, _flapPower, _flapDistance;
    [SerializeField] private bool _isLeftHand;
    private bool _isFlapping; //파리채 공격중일때는 FixedUpdate에서 충돌검사
    private LayerMask _whatIsEnemy;
    #endregion

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _attackPosTrm = transform.Find("AttackPoint");
        _initPosition = transform.position; //초기 포지션 저장해두고
        _whatIsEnemy = 1 << LayerMask.NameToLayer("Player");
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCol = GetComponent<BoxCollider2D>();
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

    #region 쇼크웨이브 공격
    public void AttackShockSequence(Vector3 targetPos, Action Callback)
    {
        Vector3 atkPos = targetPos - _attackPosTrm.localPosition; //로컬값을 빼주면 델타만큼빠진다.
        seq = DOTween.Sequence();
        //여기에 전조증상을 추가해야한다.
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
        OnAttackFeedback?.Invoke(); //카메라 흔들림 등의 피드백
        //쇼크 임팩트 재생하고
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
            if (dir.sqrMagnitude == 0)  //정가운데 맞으면 랜덤방향으로 튀도록
            {
                dir = Random.insideUnitCircle;
            }

            iKnock?.KnockBack(dir.normalized, _knockPower, 1f);
        }

    }
    #endregion

    #region 파리채 공격
    public void AttackFlapperSequence(Vector3 targetPos, Action Callback)
    {
        Vector3 atkPos = targetPos - _attackPosTrm.localPosition; //로컬값을 빼주면 델타만큼빠진다.

        seq = DOTween.Sequence();
        seq.Append(transform.DOMoveY(atkPos.y, 0.4f)); //0.4초간 주먹 앞으로 전진
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() =>
        {
            _animator.SetTrigger(_hashFlapperAttack); //손바닥 애니메이션 재생
            _isFlapping = true; //손바닥 센서 온!
            OnFlapFeedback?.Invoke(); //사운드 등 피드백 재생
        });
        float x = _isLeftHand  ? -1f : 1f;
        float targetX = transform.position.x + x * _flapDistance;
        seq.Join(transform.DOMoveX(targetX, 0.7f));
        seq.AppendInterval(0.3f);
        seq.AppendCallback(() =>
        {
            _isFlapping = false; //손바닥 센서 오프
        });
        seq.Join(transform.DOMove(_initPosition, 0.3f));
        seq.AppendCallback(() => Callback?.Invoke());
    }

    private void FixedUpdate()
    {
        //파리채 모드일때는 데미지를 가하도록
        if(_isFlapping)
        {
            Vector3 pos = _attackPosTrm.position;

            //가로 1 세로 2유닛짜리 사각형으로 검사
            Collider2D col = Physics2D.OverlapArea(pos - new Vector3(0.5f, 1f), pos + new Vector3(0.5f, 1f), _whatIsEnemy);

            if(col != null)
            {
                _isFlapping = false; //한대만 때리도록 꺼주고

                IHittable iHit = col.GetComponent<IHittable>();
                iHit.GetHit(_damage, gameObject);

                Vector3 dir = _isLeftHand ? new Vector3(-1, -1) : new Vector3(1, -1);
                IKnockBack iKnock = col.GetComponent<IKnockBack>();

                iKnock?.KnockBack(dir.normalized, _flapPower, 1f);
            }
        }
    }

    #endregion

    
    public void Regenerate(int maxHp)
    {
        
        _spriteRenderer.material.SetFloat("_Dissolve", 1);
        gameObject.SetActive(true);
        _hp = maxHp;
        _animator.SetTrigger(_hashFadeIn);
        StartCoroutine(ResetDeadParam());
    }

    IEnumerator ResetDeadParam()
    {
        yield return new WaitForSeconds(1f);
        _boxCol.enabled = true;
        _isDead = false;
    }

    public void GetHit(int damage, GameObject damageDealer)
    {
        if (_isDead) return;

        OnGetHit?.Invoke();
        _hp -= damage;

        _hitPoint = transform.position + (Vector3)Random.insideUnitCircle; //피격지점 저장

        bool isCritical = GameManager.Instance.IsCritical;
        if (isCritical)
        {
            damage = GameManager.Instance.GetCriticalDamage(damage);
        }

        PopupText damagePopup = PoolManager.Instance.Pop("PopupText") as PopupText;

        damagePopup?.Setup(damage, transform.position + new Vector3(0, 0.5f, 0), isCritical, Color.white);

        if (_hp <= 0)
        {
            KillThisHand();
        }
    }

    public void KillThisHand()
    {
        //이미 죽은 팔은 내버려둔다.
        if (gameObject.activeSelf == false) return;

        seq?.Kill();  //사망시 진행되던 시퀀스는 킬
        seq = null;

        _hp = 0;
        _isDead = true;
        OnDead?.Invoke();
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
