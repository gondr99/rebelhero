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


    private Hand _leftHand;
    public Hand LeftHand => _leftHand;
    private Hand _rightHand;
    public Hand RightHand => _rightHand;

    protected override void Awake()
    {
        base.Awake();
        _phaseData = transform.Find("AI").GetComponent<AIDemonBossPhaseData>();

        _leftHand = transform.Find("LeftHand").GetComponent<Hand>();
        _rightHand = transform.Find("RightHand").GetComponent<Hand>();

        _leftHand.InitHand(200);
        _rightHand.InitHand(200);

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
            //time = 
        };
    }
    #endregion

    protected override void Update()
    {
        currentState.UpdateState();
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
    
}
