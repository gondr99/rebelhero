using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Room : MonoBehaviour
{
    protected List<EnemySpawner> _spawnerList;
    protected List<Door> _doorList;
    protected List<Enemy> _enemyList;
    protected List<Trap> _trapList;
    public List<Door> DoorList { get => _doorList; }

    [SerializeField]
    protected bool _roomCleared = false, _isBossRoom;
    public bool IsBossRoom => _isBossRoom;
    private Boss _roomBoss;
    public Boss RoomBoss => _roomBoss;
    [SerializeField]
    private Vector3 _camOffset = new Vector3(0, 3.5f,0);

    private Transform _startPosTrm;
    public Vector3 StartPosition
    {
        get => _startPosTrm.position;
    }

    [SerializeField]
    private int _closedPortalCount;
    private int _deadEnemyCount;

    //플레이어가 룸에 들어왔을때 시작되는 매서드
    public void ActiveRoom()
    {
        _spawnerList.ForEach(x => x.ActivatePortalSensor()); //포탈센서 활성화

        if(_isBossRoom)
        {
            UIManager.Instance.EnteringBossRoom(_roomBoss, _camOffset);
        }
    }

    private bool _loadComplete = false;

    protected void Awake()
    {
        if (_loadComplete == false) LoadRoomData();
    }

    public void LoadRoomData()
    {
        if (_loadComplete == true) return;

        _spawnerList = new List<EnemySpawner>();
        transform.Find("Portals").GetComponentsInChildren<EnemySpawner>(_spawnerList);
        _closedPortalCount = 0;
        foreach (EnemySpawner es in _spawnerList)
        {
            //포탈이 닫혔을 때 해줄 일을 기록
            UnityAction closeAction = null;
            closeAction = () =>
            {
                _closedPortalCount++;
                es.OnClosePortal.RemoveListener(closeAction);
                CheckClear();
            };
            es.OnClosePortal.AddListener(closeAction);
        }
        _enemyList = new List<Enemy>();
        transform.Find("Enemies").GetComponentsInChildren<Enemy>(_enemyList);
        _deadEnemyCount = 0;

        foreach (Enemy e in _enemyList)
        {
            UnityAction dieAction = null;
            dieAction = () =>
            {
                _deadEnemyCount++;
                CheckClear();
                e.OnDie.RemoveListener(dieAction);
            };
            e.OnDie.AddListener(dieAction);
        }

        _doorList = new List<Door>();
        transform.Find("Doors").GetComponentsInChildren<Door>(_doorList);

        _startPosTrm = transform.Find("StartPosition");

        //모든 트랩 가져와서 저장
        _trapList = new List<Trap>();
        transform.Find("Traps").GetComponentsInChildren<Trap>(_trapList);

        _loadComplete = true;

        if(_isBossRoom)
        {
            _roomBoss = transform.Find("Enemies").GetComponentInChildren<Boss>();
        }
    }

    public void CheckClear()
    {
        if (_deadEnemyCount >= _enemyList.Count && _closedPortalCount >= _spawnerList.Count)
        {
            ClearRoom(); //모든 문 개방
            _trapList.ForEach(x => x.DisableTrap()); //모든 트랩 비활성화
        }
    }

    protected virtual void ClearRoom()
    {
        //문열고 다음방 만들어주고 등 처리
        OpenAllDoor();
        _roomCleared = true;
    }

    protected void OpenAllDoor()
    {
        _doorList.ForEach(x => x.OpenDoor());
    }
}
