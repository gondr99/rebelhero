using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    protected List<EnemySpawner> _spawnerList;
    protected List<Door> _doorList;
    public List<Door> DoorList { get => _doorList; }

    [SerializeField]
    protected bool _roomCleared = false;

    private Transform _startPosTrm;
    public Vector3 StartPosition
    {
        get => _startPosTrm.position;
    }

    private int _closedPortalCount;
    //플레이어가 룸에 들어왔을때 시작되는 매서드
    public void ActiveRoom()
    {
        _spawnerList.ForEach(x => x.ActivatePortalSensor()); //포탈센서 활성화
    }

    protected virtual void Awake()
    {
        _spawnerList = new List<EnemySpawner>();
        transform.Find("Portals").GetComponentsInChildren<EnemySpawner>(_spawnerList);
        _closedPortalCount = 0;
        foreach (EnemySpawner es in _spawnerList)
        {
            //포탈이 닫혔을 때 해줄 일을 기록
            es.OnClosePortal.AddListener(() =>
            {
                _closedPortalCount++;
                if(_closedPortalCount >= _spawnerList.Count)
                {
                    OpenAllDoor(); //모든 문 개방
                }
            });
        }

        _doorList = new List<Door>();
        transform.Find("Doors").GetComponentsInChildren<Door>(_doorList);

        _startPosTrm = transform.Find("StartPosition");
    }

    protected virtual void ClearRoom()
    {
        //문열고 다음방 만들어주고 등 처리
        OpenAllDoor();

    }

    protected void OpenAllDoor()
    {
        _doorList.ForEach(x => x.OpenDoor());
    }
}
