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
        //������ ��ġ�� ������ŭ ��� �ش� ��ġ�� �ڷ�ƾ���� ��Ż�� ��ȯ
        Vector3[] selectPos = new Vector3[_summonCnt]; //��ȯ�� ������ŭ ���� ��ġ �ľ�

        int[] posArr = new int[_summonPoints.Length]; //posArr �� 0,1,2,3 �ִ´�.
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

        //������� ���� selectPos�� ��ǥ ���� �Ϸ�
        
        for(int i = 0; i < selectPos.Length; i++)
        {
            EnemySpawner es = PoolManager.Instance.Pop("SmallPortal") as EnemySpawner;
            es.transform.position = selectPos[i];
            es.SetPortalData(cnt:5, passive: true); //5������ �нú�� ��ȯ

            //��ȯ�� ��Ż�� �극�ο��� ������� �Ҷ� ��������.
            UnityAction action = () =>
            {
                es.KillAllEnemyFromThisPortal();
            };

            _aiBrain.OnKillAllEnemies.AddListener(action);

            //��Ż�� ������ �̺�Ʈ ���� ����
            es.OnClosePortal.AddListener(() =>
            {
                _aiBrain.OnKillAllEnemies.RemoveListener(action);
            });

            yield return new WaitForSeconds(1f); //1�� ��������
        }

        Callback?.Invoke();
    }
}
