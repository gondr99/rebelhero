using System;
using System.Collections;
using System.Collections.Generic;
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

    //이쪽 파트는 나중에 SO로
    [SerializeField]
    private float _timer = 10f; //피격상태가 지속되는 시간
    private float _generateTimer = 0f;
    private bool _isNeutral = false;
    private int _handHP = 50;
    private int _bossHP = 500;
    private int _neutralCnt = 100; //100만큼의 무력 수치

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
            time = 5f
        };
        _attackDic.Add(AttackType.Fireball, fireballData);

        EnemyAttackData shockPunchData = new EnemyAttackData
        {
            atk = attackTrm.GetComponent<ShockPunchAttack>(),
            animAction = OnHandAttackCast,
            time = 4f
        };
        _attackDic.Add(AttackType.ShockPunch, shockPunchData);

        EnemyAttackData flapperPunchData = new EnemyAttackData
        {
            atk = attackTrm.GetComponent<FlapperPunchAttack>(),
            animAction = OnHandAttackCast,
            time = 4f
        };
        _attackDic.Add(AttackType.FlapperPunch, flapperPunchData);

        EnemyAttackData summonPortalData = new EnemyAttackData
        {
            atk = attackTrm.GetComponent<SummonPortalAttack>(),
            animAction = OnFireBallCast,
            time = 10f
        };
        _attackDic.Add(AttackType.SummonPortal, summonPortalData);
    }
    #endregion

    protected override void Update()
    {
        currentState.UpdateState();

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

        if(Input.GetKeyDown(KeyCode.P))
        {
            //모든 적 사망처리 치트키
            OnKillAllEnemies?.Invoke();
        }
        
    }

    IEnumerator DelayForNeutral()
    {
        yield return new WaitForSeconds(5f);
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
            atkData.atk.Attack(() => {
                _phaseData.idleTime = atkData.time; //이시간만큼 대기후 다시 파이어볼 발사
                SetNextAttackPattern();
                fInfo.SetValue(_phaseData, false);
            });

            atkData.animAction?.Invoke();
        }
    }

    private void SetNextAttackPattern()
    {
        //공격종류 설정
        int cnt = Enum.GetValues(typeof(AttackType)).Length;
        int startNum = 0;
        if (_phaseData.HasArms == false)
            startNum = 2; //1,2번 공격은 팔이 없으면 못한다.
        int randIdx = Random.Range(startNum, cnt);

        _phaseData.nextAttackType = (AttackType)randIdx;
        
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
        yield return new WaitForSeconds(_timer);
        _generateTimer = 10f; //4초 안에 무력화 못시키면 팔 재생
        //바로 포탈 소환
        _phaseData.nextAttackType = AttackType.SummonPortal;
        _phaseData.idleTime = 0f;
        _boss.NeutralCnt = _neutralCnt;
        _boss.State = Boss.BossState.Generate;
    }
}
