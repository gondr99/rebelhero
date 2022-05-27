using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour, IAgent, IHittable, IKnockBack
{
    [SerializeField] private AgentStatusSO _agentStatusSO;
    public AgentStatusSO PlayerStatus { get => _agentStatusSO; }

    [SerializeField]
    private int _health;
    public int Health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, _agentStatusSO.maxHP);
        }
    }
    
    //사망처리 불리언 변수
    private bool _isDead = false;
    private PlayerWeapon _playerWeapon;
    public PlayerWeapon PlayerWeapon { get => _playerWeapon; }

    [field: SerializeField] public UnityEvent OnDie { get; set; }
    [field: SerializeField] public UnityEvent OnGetHit { get; set; }

    public bool IsEnemy => false;
    public Vector3 _hitPoint{ get; private set;}

    private AgentMovement _agentMovement;


    public void GetHit(int damage, GameObject damageDealer)
    {
        if (_isDead) return;

        Health -= damage;
        OnGetHit?.Invoke();
        if (Health <= 0)
        {
            OnDie?.Invoke();
            _isDead = true;
        }
    }

    private void Awake()
    {
        _playerWeapon = transform.Find("WeaponParent").GetComponent<PlayerWeapon>();
        _agentMovement = GetComponent<AgentMovement>();
    }

    private void Start()
    {
        Health = _agentStatusSO.maxHP;
    }

    public void KnockBack(Vector2 direction, float power, float duration)
    {
        _agentMovement.KnockBack(direction, power, duration);
    }
}
