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

    public UnityEvent OnShock = null; //화면흔들림과 이펙트
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
                    OnShock?.Invoke(); //쇼크
                    OnChangeInvincible?.Invoke(false);
                    break;
                case BossState.Generate:
                    OnChangeGenerateState?.Invoke(true);                    
                    _neutralBar.gameObject.SetActive(true);
                    _neutralBar.SetHealth(_neutralCnt); //무력화 수치는 50
                    break;
                case BossState.Neutral:
                    OnShock?.Invoke(); //쇼크
                    _neutralBar.gameObject.SetActive(false);
                    OnChangeGenerateState.Invoke(false);
                    OnChangeNeutral.Invoke(true);
                    break;
            }
            
        }
    }
    

    [SerializeField] //나중에 SO를 통해서 값을 넣어야 한다.
    private int _hp;
    public int HP { get=> _hp; set=> _hp = value; }
    private bool _isDead = false;
    public bool IsDead => _isDead;

    private HealthBar _neutralBar;
    private int _neutralCnt;
    public int NeutralCnt { get => _neutralCnt; set => _neutralCnt = value; }

    

    private void Awake()
    {
        _neutralBar = transform.Find("NeutralBar").GetComponent<HealthBar>();
        _neutralBar.gameObject.SetActive(false); //감춰두고
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
                KillThisBoss();
            }
        }        
    }
    
    public void KillThisBoss()
    {
        StartCoroutine(KillDelay( 2f));
    }

    IEnumerator KillDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        this.gameObject.SetActive(false);
    }
}
