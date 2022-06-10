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
        //랜덤한 위치로 갯수만큼 골라서 해당 위치에 코루틴으로 포탈을 소환
        Vector3[] selectPos = new Vector3[_summonCnt];

        yield break;
    }
}
