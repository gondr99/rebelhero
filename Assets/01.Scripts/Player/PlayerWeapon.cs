using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections.Generic;
using System;

public class PlayerWeapon : AgentWeapon
{
    //나중에 플레이어만을 위한 무기교체, 무기드랍, 무기얻기 코드가 여기 들어옵니다.
    [field: SerializeField]
    public UnityEvent<int, int> OnChangeTotalAmmo { get; set; }

    [SerializeField]
    private ReloadGagueUI _reloadUI = null;
    [SerializeField]
    private AudioClip _cannotSound = null;

    [SerializeField] private int _maxTotalAmmo = 2000; //2000발은 넘길 수 없음
    [SerializeField] private int _totalAmmo = 200; //초기 200발을 가지고 시작

    public bool AmmoFull { get => _totalAmmo == _maxTotalAmmo; }
    public int TotalAmmo
    {
        get => _totalAmmo;
        set
        {
            _totalAmmo = Mathf.Clamp(value, 0, _maxTotalAmmo);
            OnChangeTotalAmmo?.Invoke(_totalAmmo, _maxTotalAmmo);
        }
    }

    private AudioSource _audioSoruce;
    private Weapon _currentWeapon = null;

    private bool _isReloading = false;
    public bool IsReloading { get => _isReloading; }


    #region 무기 교체 관련 로직
    [SerializeField] private List<Weapon> _weaponList = new List<Weapon>();
    private Player _player;
    private int _currentWeaponIndex = 0;

    public UnityEvent<List<Weapon>, int> UpdateWeaponUI;
    public UnityEvent<bool, Action> ChangeWeaponUI;
    private bool _isChangeWeapon = false;
    #endregion

    //땅에 떨어진 무기를 주을 수 있게
    public DroppedWeapon dropWeapon = null;

    protected override void AwakeChild()
    {
        _audioSoruce = GetComponent<AudioSource>();
        _player = transform.parent.GetComponent<Player>();
    }

    private void Start()
    { 
        Weapon[] weapons = GetComponentsInChildren<Weapon>();
        for(int i = 0; i < _player.PlayerStatus.maxWeapon; i++)
        {
            if(weapons.Length <= i)
            {
                _weaponList.Add(null); //비어 있는 아이템 삽입
            }
            else
            {
                _weaponList.Add(weapons[i]); //최대 허용가능한 수치까지만 가져간다.
                weapons[i].gameObject.SetActive(false);
            }
        }
        if(_weaponList.Count > 0)
        {
            _currentWeapon = _weaponList[0];
            _currentWeapon.gameObject.SetActive(true);
            AssignWeapon();
            OnChangeTotalAmmo?.Invoke(_totalAmmo, _maxTotalAmmo);
        }

        UpdateWeaponUI?.Invoke(_weaponList, _currentWeaponIndex); //UI갱신
    }

    public void ChangeToNextWeapon(bool isPrev)
    {
        if (_isReloading == true || _weaponList.Count <= 1 || _isChangeWeapon == true)
        {
            PlayClip(_cannotSound);
            return;
        }

        _isChangeWeapon = true;
        _currentWeapon?.gameObject.SetActive(false);
        ChangeWeaponUI?.Invoke(isPrev, () =>
        {
            //여기에 무기 교체후 할 일을 써준다.
            int nextIdx = 0;
            if (isPrev)
            {
                nextIdx = _currentWeaponIndex - 1 < 0 ? _weaponList.Count - 1 : _currentWeaponIndex - 1;
            }
            else
            {
                nextIdx = (_currentWeaponIndex + 1) % _weaponList.Count;
            }

            ChangeWeapon(_weaponList[nextIdx]);
            _currentWeaponIndex = nextIdx;
            _isChangeWeapon = false;
        });
        
    }

    public override void AssignWeapon()
    {
        _weapon = _currentWeapon;
        _weaponRenderer = _weapon?.Render;
    }

    public void ReloadGun()
    {
        if (_weapon != null && !_isReloading && _totalAmmo > 0 && !_weapon.AmmoFull)
        {
            _isReloading = true;
            _weapon.StopShooting(); //재장전시 총쏘는거 멈추고
            StartCoroutine(ReloadCoroutine());
        }
        else
        {
            PlayClip(_cannotSound);
        }
    }

    private void PlayClip(AudioClip clip)
    {
        _audioSoruce.Stop();
        _audioSoruce.clip = clip;
        _audioSoruce.Play();
    }

    IEnumerator ReloadCoroutine()
    {
        _reloadUI.gameObject.SetActive(true);
        float time = 0;
        while (time <= _weapon.WeaponData.reloadTime)
        {
            _reloadUI.ReloadGagueNormal(time / _weapon.WeaponData.reloadTime);
            time += Time.deltaTime;
            yield return null;
        }
        _reloadUI.gameObject.SetActive(false);
        PlayClip(_weapon.WeaponData.reloadClip); //무기의 재장전 클립 재생
                                                 //실질적인 재장전 로직 여기에

        int reloadedAmmo = Mathf.Min(TotalAmmo, _weapon.EmptyBulletCnt); //부족한 분량과 가지고 있는 분량중에 작은거 택하고
        TotalAmmo -= reloadedAmmo;
        _weapon.Ammo += reloadedAmmo;

        _isReloading = false;
    }

    public override void Shoot()
    {
        if (_weapon == null)
        {
            PlayClip(_cannotSound);
            return;
        }
        if (_isReloading)
        {
            PlayClip(_weapon.WeaponData.outOfAmmoClip);
            return;
        }
        base.Shoot();
    }

    public void ChangeWeapon(Weapon weapon)
    {
        _currentWeapon = weapon;
        if (weapon != null)
        {
            weapon.gameObject.SetActive(true);
            weapon.ResetWeapon();
        }
        AssignWeapon(); //새롭게 바뀐무기에 맞게 렌더러 등을 변경
    }

    public void AddWeapon()
    {
        if (_isReloading) return; //재장전중일때는 불가능
        //3가지 경우에 따라 다르게 행동한다. 
        // 1. 땅에 떨어진 무기가 있고 그 위에 플레이거 있으며 내가 지금 무기를 안들고 있다면 줍는다.
        // 2. 땅에 떨어진 무기가 있고 내가 지금 무기를 들고 있다면 땅에 떨어진건 줍고, 내가 들고 있는건 버린다.
        // 3. 땅에 떨어진 무기가 없고 내가 지금 무기를 들고 있다면 버린다.

        //3번
        if(_currentWeapon != null)
        {
            DropWeapon(_currentWeapon);
            //현재 무기는 null처리해서 공백패널로 만들고
            _weaponList[_currentWeaponIndex] = null;
        }

        //땅바닥에 웨폰이 놓여있다면
        if(dropWeapon != null)
        {
            dropWeapon.transform.parent = transform;
            dropWeapon.transform.localPosition = new Vector3(0.5f, 0, 0);
            dropWeapon.transform.localRotation = Quaternion.identity;
            _currentWeapon = _weapon = dropWeapon.GetComponent<Weapon>();
            
            _weaponList[_currentWeaponIndex] = _currentWeapon;
            
            dropWeapon.PickUpWeapon();
            dropWeapon = null;
        }
        //여기까지 오면 무기정보가 변경된것이니 Update
        UpdateWeaponUI?.Invoke(_weaponList, _currentWeaponIndex);
    }

    public void DropWeapon(Weapon weapon)
    {
        _weapon = null;
        _currentWeapon = null;
        weapon.StopShooting(); //정지 , 이거 아래랑 순서바꾸면 안된다.
        weapon.transform.parent = null;

        //총구방향으로 던진다.
        Vector3 targetPosition = weapon.GetRightDirection() * 0.5f + weapon.transform.position;
        weapon.transform.rotation = Quaternion.identity;
        weapon.transform.localScale = new Vector3(1, 1, 1);

        //이부분은 향후 시퀀스로 대체된다.
        Sequence seq = DOTween.Sequence();
        seq.Append(weapon.transform.DOMove(targetPosition, 0.5f));
        seq.AppendCallback(() =>
        {
            weapon.DropWeapon.IsActive = true;
        });
        
    }
}

