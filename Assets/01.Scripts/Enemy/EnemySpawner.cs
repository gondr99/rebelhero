using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [Header("적 생성")]
    [SerializeField]
    private List<EnemyDataSO> _enemyList;
    [SerializeField]
    private int _count = 10;
    private int _spawnCount = 0;
    private int _deadCount = 0;
    [SerializeField]
    private float _minDelay = 0.8f, _maxDelay = 1.5f;

    private Animator _animator;
    private readonly int _hashOpen = Animator.StringToHash("open");
    private readonly int _hashClose = Animator.StringToHash("close");

    [Header("포탈 사용자 감지")]
    [SerializeField]
    private float _detectRadius = 5f;
    [SerializeField]
    private LayerMask _playerMask;

    [SerializeField]
    private AudioClip _openClip;
    [SerializeField]
    private AudioClip _closeClip;
    [SerializeField]
    private float _portalOpenDelay = 1f;

    private AudioSource _audioSoruce;

    private bool _isOpen = false;

    private HealthBar _healthBar;
    [SerializeField]
    private bool _sensorActive = false;


    [Header("포탈 상태 및 이벤트")]
    [SerializeField]
    private bool _passiveActive = false; //기본 액티브로 이 값이 활성화 되어 있다면 플레이어 유무와 상관없이 포탈이 활성화 된다.
    public UnityEvent OnClosePortal = null;

    private void Awake()
    {
        _animator = transform.Find("VisualSprite").GetComponent<Animator>();

        int playerLayer = LayerMask.NameToLayer("Player"); //플레이어 레이어 코드번호 알아내고
        _playerMask = 1 << playerLayer;

        _audioSoruce = GetComponent<AudioSource>();

        _healthBar = transform.Find("HealthBar").GetComponent<HealthBar>();
    }

    //포탈이 플레이어를 감지하기 시작함
    public void ActivatePortalSensor()
    {
        _sensorActive = true;
    }

    private void FixedUpdate()
    {
        if (!_isOpen && _sensorActive)
        {
            if (_passiveActive)
            {
                OpenPortal();
            }
            else
            {
                Collider2D coliider = Physics2D.OverlapCircle(transform.position, _detectRadius, _playerMask);

                if (coliider != null)
                {
                    OpenPortal();
                }
            }
        }
    }


    private void OpenPortal()
    {
        _isOpen = true;
        _animator.SetTrigger(_hashOpen);
        _audioSoruce.clip = _openClip;
        _audioSoruce.Play();

        _healthBar.SetHealth(_count); //갯수만큼 체력으로 생성

        StartCoroutine(SpawnCoroutine());
    }

    private void ClosePortal()
    {
        _animator.SetTrigger(_hashClose);
        _audioSoruce.clip = _closeClip;
        _audioSoruce.Play();

        _healthBar.gameObject.SetActive(false);
        //여기에 포탈 사라지는 코드도 추가
        StartCoroutine(DestroyPortal());
    }

    IEnumerator DestroyPortal()
    {
        yield return new WaitForSeconds(2f);
        
        OnClosePortal?.Invoke();

        Destroy(gameObject);
    }

    IEnumerator SpawnCoroutine()
    {
        yield return new WaitForSeconds(_portalOpenDelay);
        while (_spawnCount < _count)
        {
            int randomIndex = Random.Range(0, _enemyList.Count); //어떤 적을 생성할지 결정
            Vector2 randomOffset = Random.insideUnitCircle;
            randomOffset.y = -Mathf.Abs(randomOffset.y); //포탈의 방향으로 적들이 뛰어나오게
            EnemyDataSO spawnEnemyData = _enemyList[randomIndex];

            Vector3 posToSpawn = transform.position;// + (Vector3)randomOffset;

            Enemy spawnedEnemy = SpawnEnemy(posToSpawn, spawnEnemyData);

            //등장 : 이 부분은 차후에 튀어 나오는 모션으로 변경 필요함
            spawnedEnemy.SpawnInPortal(transform.position + (Vector3)randomOffset * 1f, power:2f, time:0.8f);


            //사망 처리 빼주는거 안했어.
            UnityAction deadAction = null;
            deadAction = () =>
            {
                _deadCount++;
                _healthBar.SetHealth(_count - _deadCount);
                if (_deadCount == _count)
                {
                    ClosePortal(); //주모 샷따 내려
                }
                spawnedEnemy.OnDie.RemoveListener(deadAction);
            };
            spawnedEnemy.OnDie.AddListener(deadAction);

            float waitTime = Random.Range(_minDelay, _maxDelay);
            _spawnCount++;

            yield return new WaitForSeconds(waitTime); //대기시간 이후에 생성
        }
    }

    private Enemy SpawnEnemy(Vector3 posToSpawn, EnemyDataSO spawnEnemyData)
    {
        Enemy enemy = (Enemy)PoolManager.Instance.Pop(spawnEnemyData.enemyName);
        enemy.transform.position = posToSpawn;
        return enemy;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.Selection.activeObject == gameObject)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _detectRadius);
            Gizmos.color = Color.white;
        }
    }
#endif
}
