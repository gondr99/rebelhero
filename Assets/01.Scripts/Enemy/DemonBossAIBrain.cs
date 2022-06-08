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
        RocketPunch = 0,
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

    protected override void Awake()
    {
        base.Awake();
        _phaseData = transform.Find("AI").GetComponent<AIDemonBossPhaseData>();

        Transform attackTrm = transform.Find("AttackType");

        //�̰� ���߿� SO �� ���� �� �ִ�.
        EnemyAttackData fireballData = new EnemyAttackData {
            atk = attackTrm.GetComponent<FireBallAttack>(),
            animAction = OnFireBallCast,
            time = 5f
        };
        _attackDic.Add(AttackType.Fireball, fireballData);
    }

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
