using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [Header("�� ����")]
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

    [Header("��Ż ����� ����")]
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


    [Header("��Ż ���� �� �̺�Ʈ")]
    [SerializeField]
    private bool _passiveActive = false; //�⺻ ��Ƽ��� �� ���� Ȱ��ȭ �Ǿ� �ִٸ� �÷��̾� ������ ������� ��Ż�� Ȱ��ȭ �ȴ�.
    public UnityEvent OnClosePortal = null;

    private void Awake()
    {
        _animator = transform.Find("VisualSprite").GetComponent<Animator>();

        int playerLayer = LayerMask.NameToLayer("Player"); //�÷��̾� ���̾� �ڵ��ȣ �˾Ƴ���
        _playerMask = 1 << playerLayer;

        _audioSoruce = GetComponent<AudioSource>();

        _healthBar = transform.Find("HealthBar").GetComponent<HealthBar>();
    }

    //��Ż�� �÷��̾ �����ϱ� ������
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

        _healthBar.SetHealth(_count); //������ŭ ü������ ����

        StartCoroutine(SpawnCoroutine());
    }

    private void ClosePortal()
    {
        _animator.SetTrigger(_hashClose);
        _audioSoruce.clip = _closeClip;
        _audioSoruce.Play();

        _healthBar.gameObject.SetActive(false);
        //���⿡ ��Ż ������� �ڵ嵵 �߰�
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
            int randomIndex = Random.Range(0, _enemyList.Count); //� ���� �������� ����
            Vector2 randomOffset = Random.insideUnitCircle;
            randomOffset.y = -Mathf.Abs(randomOffset.y); //��Ż�� �������� ������ �پ����
            EnemyDataSO spawnEnemyData = _enemyList[randomIndex];

            Vector3 posToSpawn = transform.position;// + (Vector3)randomOffset;

            Enemy spawnedEnemy = SpawnEnemy(posToSpawn, spawnEnemyData);

            //���� : �� �κ��� ���Ŀ� Ƣ�� ������ ������� ���� �ʿ���
            spawnedEnemy.SpawnInPortal(transform.position + (Vector3)randomOffset * 1f, power:2f, time:0.8f);


            //��� ó�� ���ִ°� ���߾�.
            UnityAction deadAction = null;
            deadAction = () =>
            {
                _deadCount++;
                _healthBar.SetHealth(_count - _deadCount);
                if (_deadCount == _count)
                {
                    ClosePortal(); //�ָ� ���� ����
                }
                spawnedEnemy.OnDie.RemoveListener(deadAction);
            };
            spawnedEnemy.OnDie.AddListener(deadAction);

            float waitTime = Random.Range(_minDelay, _maxDelay);
            _spawnCount++;

            yield return new WaitForSeconds(waitTime); //���ð� ���Ŀ� ����
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
