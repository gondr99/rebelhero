using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonPortalAttack : DemonBossAttack
{
    private DemonBossAIBrain _brain;

    [SerializeField]
    private int _summonCnt = 2;
    private Vector3[] _summonPoints;

    protected override void Awake()
    {
        base.Awake();
        _brain = transform.parent.GetComponent<DemonBossAIBrain>();
        Transform summonTrm = transform.Find("SummonArea");

        _summonPoints = new Vector3[summonTrm.childCount];
        
        for(int i = 0; i < _summonPoints.Length; i++)
        {
            _summonPoints[i] = summonTrm.GetChild(i).position;
        }

        _summonCnt = Mathf.Clamp(_summonCnt, 0, _summonPoints.Length);
    }

    public override void Attack(Action Callback)
    {
        StartCoroutine(SummonPortal(Callback));
    }

    IEnumerator SummonPortal(Action Callback)
    {
        //������ ��ġ�� ������ŭ ��� �ش� ��ġ�� �ڷ�ƾ���� ��Ż�� ��ȯ
        Vector3[] selectPos = new Vector3[_summonCnt];

        yield break;
    }
}
