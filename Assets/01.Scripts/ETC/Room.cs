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
    //�÷��̾ �뿡 �������� ���۵Ǵ� �ż���
    public void ActiveRoom()
    {
        _spawnerList.ForEach(x => x.ActivatePortalSensor()); //��Ż���� Ȱ��ȭ
    }

    protected virtual void Awake()
    {
        _spawnerList = new List<EnemySpawner>();
        transform.Find("Portals").GetComponentsInChildren<EnemySpawner>(_spawnerList);
        _closedPortalCount = 0;
        foreach (EnemySpawner es in _spawnerList)
        {
            //��Ż�� ������ �� ���� ���� ���
            es.OnClosePortal.AddListener(() =>
            {
                _closedPortalCount++;
                if(_closedPortalCount >= _spawnerList.Count)
                {
                    OpenAllDoor(); //��� �� ����
                }
            });
        }

        _doorList = new List<Door>();
        transform.Find("Doors").GetComponentsInChildren<Door>(_doorList);

        _startPosTrm = transform.Find("StartPosition");
    }

    protected virtual void ClearRoom()
    {
        //������ ������ ������ְ� �� ó��
        OpenAllDoor();

    }

    protected void OpenAllDoor()
    {
        _doorList.ForEach(x => x.OpenDoor());
    }
}
