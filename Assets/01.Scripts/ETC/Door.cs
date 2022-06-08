using UnityEngine;
using static Define;

public class Door : MonoBehaviour
{
    private Transform _openTrm;
    private Transform _closeTrm;
    [SerializeField]
    private bool _isOpen = false;

    private AudioSource _audioSource;

    [SerializeField]
    private RoomIconSO _iconSo;
    private SpriteRenderer _iconSprite;
    
    [SerializeField]
    private RoomType _nextRoom;
    public RoomType nextRoomType { 
        get => _nextRoom; 
        set {
            _nextRoom = value;
            _iconSprite.sprite = _iconSo.sprites[(int)value];
        } 
    }


    private void Awake()
    {
        _openTrm = transform.Find("Open");
        _closeTrm = transform.Find("Closed");
        _iconSprite = transform.Find("Category/Icon").GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //�����ְ� ���� ��Ҵٸ� ���������� �ѱ���
        if (_isOpen)
        {
            //���������� �Ѿ�� ��ƾ �ʿ�
            GameManager.Instance.LoadNextRoom(nextRoomType);
        }
    }

    public void OpenDoor()
    {
        _isOpen = true;
        _openTrm.gameObject.SetActive(_isOpen);
        _closeTrm.gameObject.SetActive(!_isOpen);
        _audioSource.Play();
    }

    public void CloseDoor()
    {
        _isOpen = false;
        _openTrm.gameObject.SetActive(_isOpen);
        _closeTrm.gameObject.SetActive(!_isOpen);
    }
}
