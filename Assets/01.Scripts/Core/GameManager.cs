using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using static Define;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private TextureParticleManager _textureParticleManagerPrefab;
    [SerializeField] private Texture2D cursorTexture = null;
    [SerializeField] private PoolingListSO _poollingList;

    private Transform _playerTrm;
    public Transform PlayerTrm
    {
        get
        {
            if (_playerTrm == null)
            {
                GetPlayerFromHeirachy();
            }
            return _playerTrm;
        }
    }
    private Player _player;
    public AgentStatusSO PlayerStatus
    {
        get
        {
            if (_player == null)
            {
                _player = PlayerTrm.GetComponent<Player>();
            }
            return _player.PlayerStatus;
        }
    }

    public bool IsCritical => Random.value < PlayerStatus.critical;
    public int GetCriticalDamage (int damage)
    {
        float ratio = Random.Range(PlayerStatus.criticalMinDamage, PlayerStatus.criticalMaxDamage);
        damage = Mathf.CeilToInt((float)damage * ratio);
        return damage;
    }

    //public float CriticalChance { get => PlayerStatus.critical; } 
    //public float CriticalMinDamage { get => PlayerStatus.criticalMinDamage; }
    //public float CriticalMaxDamage { get => PlayerStatus.criticalMaxDamage; }
    private void GetPlayerFromHeirachy()
    {
        //이부분은 향후 변경예정
        _playerTrm = GameObject.FindGameObjectWithTag("Player").transform;
    }


    public UnityEvent<int> OnCoinUpdate = null;
    private int _coinCnt;
    public int Coin
    {
        get => _coinCnt;
        set
        {
            _coinCnt = value;
            OnCoinUpdate?.Invoke(_coinCnt);
        }
    }

    [Header("스테이지 데이터들")]
    public List<RoomListSO> stages;

    private Room _currentRoom = null;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple Gamemanager is running");
        }
        Instance = this;

        PoolManager.Instance = new PoolManager(transform);

        GameObject timeController = new GameObject("TimeController");
        timeController.transform.parent = transform.parent;
        TimeController.Instance = timeController.AddComponent<TimeController>();

        Instantiate(_textureParticleManagerPrefab, transform.parent);

        UIManager.Instance = new UIManager(); //UI 매니저 생성

        RoomManager.Instance = new RoomManager(); //룸매니저 생성
        int bossCnt = Random.Range(8, 10);
        RoomManager.Instance.InitStage(stages[0], bossCnt); //0 스테이지부터 로딩

        //최초 생성된 룸 가져올 일이 있다면
        _currentRoom = GameObject.FindObjectOfType<Room>();
        if (_currentRoom == null)
        {
            //생성된게 없으면 생성
            Room room = RoomManager.Instance.LoadStartRoom();
            room.LoadRoomData();
            ChangeRoom(room);
        }else
        {
            //이미 생성되어 있는 룸이 있다면 셋팅
            _currentRoom.LoadRoomData();
            PlayerTrm.position = _currentRoom.StartPosition;
            RoomManager.Instance.SetRoomDoorDestination(_currentRoom);
            _currentRoom.ActiveRoom();
        }

        //번역을 위한 로케일 설정
        TextManager.Instance = new TextManager();
        TextManager.Instance.Init();

        SetCursorIcon();
        CreatePool();
    }

    private void CreatePool()
    {
        foreach (PoolingPair pair in _poollingList.list)
        {
            PoolManager.Instance.CreatePool(pair.prefab, pair.poolCnt);
        }
    }

    private void SetCursorIcon()
    {
        Cursor.SetCursor(cursorTexture,
            new Vector2(cursorTexture.width / 2f, cursorTexture.height / 2f),
            CursorMode.Auto);
    }

    public void LoadNextRoom(RoomType type)
    {
        Room room = RoomManager.Instance.LoadNextRoom(type);
        //여기엔 화면 전환 장치가 필요하다.

        ChangeRoom(room);
    }

    //새로운 방으로 교체
    private void ChangeRoom(Room newRoom)
    {
        if(_currentRoom != null)
            Destroy(_currentRoom.gameObject);

        newRoom.transform.position = Vector3.zero;
        PlayerTrm.position = newRoom.StartPosition;
        _currentRoom = newRoom;
        _currentRoom.ActiveRoom(); //룸 활성화
    }
}
