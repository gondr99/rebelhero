using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

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
        Vector3[] selectPos = new Vector3[_summonCnt]; //소환할 갯수만큼 랜덤 위치 파악

        int[] posArr = new int[_summonPoints.Length]; //posArr 에 0,1,2,3 넣는다.
        for(int i = 0; i < posArr.Length; i++)
        {
            posArr[i] = i;
        }

        for(int i = 0; i < selectPos.Length; i++)
        {
            int idx = Random.Range(0, posArr.Length - i);  // 0, 1, 2, 3
            selectPos[i] = _summonPoints[posArr[idx]];

            posArr[idx] = posArr[posArr.Length - i - 1]; 
        }

        //여기까지 오면 selectPos에 좌표 저장 완료
        
        for(int i = 0; i < selectPos.Length; i++)
        {
            EnemySpawner es = PoolManager.Instance.Pop("SmallPortal") as EnemySpawner;
            es.transform.position = selectPos[i];
            es.SetPortalData(cnt:5, passive: true); //5마리를 패시브로 소환

            //소환된 포탈은 브레인에서 닫히라고 할때 닫혀야함.
            UnityAction action = () =>
            {
                es.KillAllEnemyFromThisPortal();
            };

            _aiBrain.OnKillAllEnemies.AddListener(action);

            //포탈이 닫힐때 이벤트 구독 중지
            es.OnClosePortal.AddListener(() =>
            {
                _aiBrain.OnKillAllEnemies.RemoveListener(action);
            });

            yield return new WaitForSeconds(1f); //1초 간격으로
        }

        Callback?.Invoke();
    }
}
