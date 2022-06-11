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

    public UnityEvent OnFireBallCast = null;  //���̾ ĳ���� �̺�Ʈ
    public UnityEvent OnHandAttackCast = null; //�ָ԰��� �����ϱ��� �̺�Ʈ
    public UnityEvent OnKillAllEnemies = null;


    private Hand _leftHand;
    public Hand LeftHand => _leftHand;
    private Hand _rightHand;
    public Hand RightHand => _rightHand;
    private Boss _boss;

    //���� ��Ʈ�� ���߿� SO��
    [SerializeField]
    private float _timer = 10f; //�ǰݻ��°� ���ӵǴ� �ð�
    private float _generateTimer = 0f;
    private bool _isNeutral = false;
    private int _handHP = 50;
    private int _bossHP = 500;
    private int _neutralCnt = 100; //100��ŭ�� ���� ��ġ

    protected override void Awake()
    {
        base.Awake();
        _phaseData = transform.Find("AI").GetComponent<AIDemonBossPhaseData>();

        _leftHand = transform.Find("LeftHand").GetComponent<Hand>();
        _rightHand = transform.Find("RightHand").GetComponent<Hand>();

        //�̺κ��� ���� SO�� ó���ؾ� ��.
        _leftHand.InitHand(_handHP);
        _rightHand.InitHand(_handHP);

        _boss = GetComponent<Boss>();
        _boss.HP = _bossHP; //���� ü�� 200���� ���� .

        SetDictionary();//���ݰ��� Dictionary ����
        
    }

    #region ������ �������� ���� ���� �ϴ� ��
    private void SetDictionary()
    {
        Transform attackTrm = transform.Find("AttackType");
        //�̰� ���߿� SO �� ���� �� �ִ�.
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
                SetInvincible(); //�������·� ��ȯ
            }
        }

        //����ȭ ���ٸ� ����ȭ �ð� ������ �ٽ� �������·� ��ȯ
        if(_boss.State == Boss.BossState.Neutral && _isNeutral == false)
        {
            _isNeutral = true;
            OnKillAllEnemies?.Invoke();
            StartCoroutine(DelayForNeutral());
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            //��� �� ���ó�� ġƮŰ
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
        _boss.State = Boss.BossState.Invincible; //�������·� ��ȯ
        _leftHand.Regenerate(_handHP);
        _rightHand.Regenerate(_handHP);
        _phaseData.hasLeftArm = true;
        _phaseData.hasRightArm = true;
        _phaseData.nextAttackType = AttackType.SummonPortal;
        _phaseData.idleTime = 1f; //1���� ��ȯ
    }

    public void Attack(AttackType type)
    {
        FieldInfo fInfo = typeof(AIDemonBossPhaseData).GetField(type.ToString(), BindingFlags.Public | BindingFlags.Instance);
        fInfo.SetValue(_phaseData, true);
        
        OnFireBallCast?.Invoke(); //ĳ���� ���� �ִϸ��̼� �ִٸ� ���

        EnemyAttackData atkData = null;
        _attackDic.TryGetValue(type,out atkData);

        if(atkData != null)
        {
            atkData.atk.Attack(() => {
                _phaseData.idleTime = atkData.time; //�̽ð���ŭ ����� �ٽ� ���̾ �߻�
                SetNextAttackPattern();
                fInfo.SetValue(_phaseData, false);
            });

            atkData.animAction?.Invoke();
        }
    }

    private void SetNextAttackPattern()
    {
        //�������� ����
        int cnt = Enum.GetValues(typeof(AttackType)).Length;
        int startNum = 0;
        if (_phaseData.HasArms == false)
            startNum = 2; //1,2�� ������ ���� ������ ���Ѵ�.
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

        //��� ���� �Ҿ��ٸ� �ٿ���°� �Ǹ鼭 �ǰݰ�������.
        if(_phaseData.HasArms == false)
        {
            _boss.State = Boss.BossState.Damageable;
            StartCoroutine(DelayToGenerateArm());
        }
    }

    IEnumerator DelayToGenerateArm()
    {
        yield return new WaitForSeconds(_timer);
        _generateTimer = 10f; //4�� �ȿ� ����ȭ ����Ű�� �� ���
        //�ٷ� ��Ż ��ȯ
        _phaseData.nextAttackType = AttackType.SummonPortal;
        _phaseData.idleTime = 0f;
        _boss.NeutralCnt = _neutralCnt;
        _boss.State = Boss.BossState.Generate;
    }
}
