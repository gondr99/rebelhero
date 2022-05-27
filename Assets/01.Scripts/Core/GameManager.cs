using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
            if(_player == null)
            {
                _player = PlayerTrm.GetComponent<Player>();
            }
            return _player.PlayerStatus;
        }
    }

    public float CriticalChance { get => PlayerStatus.critical; } 
    public float CriticalMinDamage { get => PlayerStatus.criticalMinDamage; }
    public float CriticalMaxDamage { get => PlayerStatus.criticalMaxDamage; }
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
}
