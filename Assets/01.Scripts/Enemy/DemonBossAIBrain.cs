using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class DemonBossAIBrain : EnemyAIBrain
{
    public enum AttackType
    {
        FlapperPunch = 0,
        ShockPunch = 1,
        Fireball = 2,
        SummonPortal = 3,
    }

    public class EnemyAttackData
    {
        public DemonBossAttack atk;
        public UnityEvent animAction;
        public float time;
    }

    public Dictionary<AttackType, EnemyAttackData> _attackDic = new Dictionary<AttackType, EnemyAttackData>();
    

    protected AIDemonBossPhaseData _phaseData;
    public AIDemonBossPhaseData PhaseData => _phaseData;

    public UnityEvent OnFireBallCast = null;  //파이어볼 캐스팅 이벤트
    public UnityEvent OnHandAttackCast = null; //주먹관련 공격하기전 이벤트
    public UnityEvent OnKillAllEnemies = null;


    private Hand _leftHand;
    public Hand LeftHand => _leftHand;
    private Hand _rightHand;
    public Hand RightHand => _rightHand;
    private Boss _boss;

    private SummonPortalAttack _summonPortalAttack; //포탈 공격은 패턴을 정할 때 필요하니 가져와둔다
    private float _currentPortalCoolTime = 0;

    //이쪽 파트는 나중에 SO로
    [SerializeField]
    private float _dealTimer = 10f, _generateTimerMax = 8f, _neutralTime = 10f, _portalCoolTime = 10f;
    //피격상태가 지속되는 시간, 8초안에 무력화, 무력화 시간

    private float _generateTimer = 0f;
    private bool _isNeutral = false;
    private int _handHP = 50;
    private int _bossHP = 500;
    private int _neutralCnt = 100; //100만큼의 무력 수치

    private Queue<AttackType> _atkQueue;

    protected override void Awake()
    {
        base.Awake();
        _phaseData = transform.Find("AI").GetComponent<AIDemonBossPhaseData>();

        _leftHand = transform.Find("LeftHand").GetComponent<Hand>();
        _rightHand = transform.Find("RightHand").GetComponent<Hand>();

        //이부분은 전부 SO로 처리해야 해.
        _leftHand.InitHand(_handHP);
        _rightHand.InitHand(_handHP);

        _boss = GetComponent<Boss>();
        _boss.HP = _bossHP; //보스 체력 200으로 설정 .

        _atkQueue = new Queue<AttackType>(); //공격타입 설정 큐

        _summonPortalAttack = transform.Find("AttackType").GetComponent<SummonPortalAttack>();
        SetDictionary();//공격관련 Dictionary 설정
        
    }

    #region 보스의 공격패턴 관련 셋팅 하는 곳
    private void SetDictionary()
    {
        Transform attackTrm = transform.Find("AttackType");
        //이건 나중에 SO 로 만들 수 있다.
        EnemyAttackData fireballData = new EnemyAttackData
        {
            atk = attackTrm.GetComponent<FireBallAttack>(),
            animAction = OnFireBallCast,
            time = 1f
        };
        _attackDic.Add(AttackType.Fireball, fireballData);

        EnemyAttackData shockPunchData = new EnemyAttackData
        {
            atk = attackTrm.GetComponent<ShockPunchAttack>(),
            animAction = OnHandAttackCast,
            time = 2f
        };
        _attackDic.Add(AttackType.ShockPunch, shockPunchData);

        EnemyAttackData flapperPunchData = new EnemyAttackData
        {
            atk = attackTrm.GetComponent<FlapperPunchAttack>(),
            animAction = OnHandAttackCast,
            time = 2f
        };
        _attackDic.Add(AttackType.FlapperPunch, flapperPunchData);

        EnemyAttackData summonPortalData = new EnemyAttackData
        {
            atk = attackTrm.GetComponent<SummonPortalAttack>(),
            animAction = OnFireBallCast,
            time = 5f
        };
        _attackDic.Add(AttackType.SummonPortal, summonPortalData);
    }
    #endregion

    protected override void Update()
    {
        //죽었다면 아무것도 하지 않는다.
        if (_boss.IsDead == true) return;  

        if (_boss.State == Boss.BossState.Generate)
        {
            _generateTimer -= Time.deltaTime;
            if (_generateTimer <= 0)
            {
                _generateTimer = 0;
                SetInvincible(); //무적상태로 전환
            }
        }

        //무력화 들어갔다면 무력화 시간 감소후 다시 무적상태로 전환
        if(_boss.State == Boss.BossState.Neutral && _isNeutral == false)
        {
            _isNeutral = true;
            OnKillAllEnemies?.Invoke();
            StartCoroutine(DelayForNeutral());
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            //모든 적 사망처리 치트키
            OnKillAllEnemies?.Invoke();
        }

        if (_isNeutral) return;

        //포탈 쿨타임이 돌고 있다면 포탈 쿨
        if (_currentPortalCoolTime > 0 )
        {
            _currentPortalCoolTime -= Time.deltaTime;
        }

        currentState.UpdateState();
    }

    IEnumerator DelayForNeutral()
    {
        _phaseData.idleTime = _neutralTime + 0.5f; //무력시간만큼 아이들

        yield return new WaitForSeconds(_neutralTime);
        SetInvincible();
        _isNeutral = false;
    }

    private void SetInvincible()
    {
        _boss.State = Boss.BossState.Invincible; //무적상태로 전환
        _leftHand.Regenerate(_handHP);
        _rightHand.Regenerate(_handHP);
        _phaseData.hasLeftArm = true;
        _phaseData.hasRightArm = true;
        _phaseData.nextAttackType = AttackType.SummonPortal;
        _phaseData.idleTime = 1f; //1초후 소환
    }

    public void Attack(AttackType type)
    {
        FieldInfo fInfo = typeof(AIDemonBossPhaseData).GetField(type.ToString(), BindingFlags.Public | BindingFlags.Instance);
        fInfo.SetValue(_phaseData, true);
        
        OnFireBallCast?.Invoke(); //캐스팅 관련 애니메이션 있다면 재생

        EnemyAttackData atkData = null;
        _attackDic.TryGetValue(type,out atkData);

        if(atkData != null)
        {
            atkData.atk.Attack((result) => {
                _phaseData.idleTime = result == true ? atkData.time : 0.2f; //공격 실패시 0.2초 이내로 다음공격 수행                
                SetNextAttackPattern();
                fInfo.SetValue(_phaseData, false);
            });

            atkData.animAction?.Invoke();
        }
    }

    private void SetNextAttackPattern()
    {
        //공격종류 설정
        if(_summonPortalAttack.summonedPortalCnt <= 0 && _currentPortalCoolTime <= 0) //현재 생성된 포탈이 없다면 바로 포탈 생성
        {
            _phaseData.nextAttackType = AttackType.SummonPortal;
            _currentPortalCoolTime = _portalCoolTime;
            return;
        }

        if(_atkQueue.Count == 0)
        {
            ShuffleAttackType();
        }
        
        _phaseData.nextAttackType = _atkQueue.Dequeue();        
    }

    private void ShuffleAttackType()
    {
        //여기를 바꾸면 단순 랜덤이 아닌 패턴화된 공격을 할 수도 있다.
        Array types = Enum.GetValues(typeof(AttackType));

        List<AttackType> list = new List<AttackType>();
        foreach(AttackType t in types)
        {
            list.Add(t);
        }
        
        for(int i = 0; i < list.Count; i++)
        {
            int idx = Random.Range(0, list.Count - i);
            _atkQueue.Enqueue(list[idx]);
            list[idx] = list[list.Count - i - 1];            
        }
    }

    public void LostArm(bool isLeft)
    {
        if(isLeft)
        {
            _phaseData.hasLeftArm = false;
        }
        else
        {
            _phaseData.hasRightArm = false;
        }

        //모든 팔을 잃었다면 다운상태가 되면서 피격가능해짐.
        if(_phaseData.HasArms == false)
        {
            _boss.State = Boss.BossState.Damageable;
            StartCoroutine(DelayToGenerateArm());
        }
    }

    IEnumerator DelayToGenerateArm()
    {
        yield return new WaitForSeconds(_dealTimer);
        _generateTimer = _generateTimerMax; //8초 안에 무력화 못시키면 팔 재생
        //바로 포탈 소환
        _phaseData.nextAttackType = AttackType.SummonPortal;
        _phaseData.idleTime = 0f;
        _boss.NeutralCnt = _neutralCnt;
        _boss.State = Boss.BossState.Generate;
    }

    public void BossDead()
    {
        OnKillAllEnemies?.Invoke();
    }
}
