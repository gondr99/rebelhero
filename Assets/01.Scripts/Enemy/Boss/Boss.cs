using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Boss : MonoBehaviour, IHittable
{
    public bool IsEnemy => true;

    public Vector3 _hitPoint { get; set; }

    public UnityEvent<bool> OnChangeNeutral = null;
    public UnityEvent<bool> OnChangeGenerateState = null;

    public UnityEvent<bool> OnChangeInvincible = null;

    public UnityEvent OnShock = null; //ȭ����鸲�� ����Ʈ
    public UnityEvent OnDead = null;

    public enum BossState
    {
        Invincible,
        Damageable,
        Generate,
        Neutral
    }
    private BossState _state;
    public BossState State
    {
        get => _state;
        set
        {
            _state = value;
            switch(_state)
            {
                case BossState.Invincible:
                    OnChangeInvincible?.Invoke(true);
                    OnChangeGenerateState.Invoke(false);
                    OnChangeNeutral.Invoke(false);
                    _neutralBar.gameObject.SetActive(false);
                    break;
                case BossState.Damageable:
                    OnShock?.Invoke(); //��ũ
                    OnChangeInvincible?.Invoke(false);
                    break;
                case BossState.Generate:
                    OnChangeGenerateState?.Invoke(true);                    
                    _neutralBar.gameObject.SetActive(true);
                    _neutralBar.SetHealth(_neutralCnt); //����ȭ ��ġ�� 50
                    break;
                case BossState.Neutral:
                    OnShock?.Invoke(); //��ũ
                    _neutralBar.gameObject.SetActive(false);
                    OnChangeGenerateState.Invoke(false);
                    OnChangeNeutral.Invoke(true);
                    break;
            }
            
        }
    }
    

    [SerializeField] //���߿� SO�� ���ؼ� ���� �־�� �Ѵ�.
    private int _hp;
    public int HP { get=> _hp; set=> _hp = value; }
    private bool _isDead = false;

    private HealthBar _neutralBar;
    private int _neutralCnt;
    public int NeutralCnt { get => _neutralCnt; set => _neutralCnt = value; }

    

    private void Awake()
    {
        _neutralBar = transform.Find("NeutralBar").GetComponent<HealthBar>();
        _neutralBar.gameObject.SetActive(false); //����ΰ�
    }

    private void Start()
    {
        State = BossState.Invincible;
    }
        

    public void GetHit(int damage, GameObject damageDealer)
    {
        if (_isDead) return;

        if(State == BossState.Invincible)
        {
            PopupText damagePopup = PoolManager.Instance.Pop("PopupText") as PopupText;
            string localeText = TextManager.Instance.LocaleString("invincible");
            damagePopup?.Setup(localeText, transform.position + new Vector3(0, 0.5f, 0), Color.white, 12f);
            return;
        }

        bool critical = GameManager.Instance.IsCritical;
        if(critical)
        {
            damage = GameManager.Instance.GetCriticalDamage(damage);
        }

        PopupText dPopup = PoolManager.Instance.Pop("PopupText") as PopupText;

        if(State == BossState.Generate)
        {
            dPopup?.Setup(damage, transform.position + new Vector3(0, 0.5f, 0), false, Color.yellow);
            _neutralCnt -= damage;
            if(_neutralCnt <= 0) {
                _neutralCnt = 0;
                State = BossState.Neutral;
            }
            _neutralBar.SetHealth(_neutralCnt);
        }
        else
        {
            dPopup?.Setup(damage, transform.position + new Vector3(0, 0.5f, 0), critical, Color.white);
            _hp -= damage;

            if (_hp <= 0)
            {
                _isDead = true;
                OnDead?.Invoke();
            }
        }        
    }
    
}