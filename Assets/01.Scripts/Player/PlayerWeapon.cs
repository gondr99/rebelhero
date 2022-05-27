using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections.Generic;
using System;

public class PlayerWeapon : AgentWeapon
{
    //���߿� �÷��̾�� ���� ���ⱳü, ������, ������ �ڵ尡 ���� ���ɴϴ�.
    [field: SerializeField]
    public UnityEvent<int, int> OnChangeTotalAmmo { get; set; }

    [SerializeField]
    private ReloadGagueUI _reloadUI = null;
    [SerializeField]
    private AudioClip _cannotSound = null;

    [SerializeField] private int _maxTotalAmmo = 2000; //2000���� �ѱ� �� ����
    [SerializeField] private int _totalAmmo = 200; //�ʱ� 200���� ������ ����

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


    #region ���� ��ü ���� ����
    [SerializeField] private List<Weapon> _weaponList = new List<Weapon>();
    private Player _player;
    private int _currentWeaponIndex = 0;

    public UnityEvent<List<Weapon>, int> UpdateWeaponUI;
    public UnityEvent<bool, Action> ChangeWeaponUI;
    private bool _isChangeWeapon = false;
    #endregion

    //���� ������ ���⸦ ���� �� �ְ�
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
                _weaponList.Add(null); //��� �ִ� ������ ����
            }
            else
            {
                _weaponList.Add(weapons[i]); //�ִ� ��밡���� ��ġ������ ��������.
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

        UpdateWeaponUI?.Invoke(_weaponList, _currentWeaponIndex); //UI����
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
            //���⿡ ���� ��ü�� �� ���� ���ش�.
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
            _weapon.StopShooting(); //�������� �ѽ�°� ���߰�
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
        PlayClip(_weapon.WeaponData.reloadClip); //������ ������ Ŭ�� ���
                                                 //�������� ������ ���� ���⿡

        int reloadedAmmo = Mathf.Min(TotalAmmo, _weapon.EmptyBulletCnt); //������ �з��� ������ �ִ� �з��߿� ������ ���ϰ�
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
        AssignWeapon(); //���Ӱ� �ٲ﹫�⿡ �°� ������ ���� ����
    }

    public void AddWeapon()
    {
        if (_isReloading) return; //���������϶��� �Ұ���
        //3���� ��쿡 ���� �ٸ��� �ൿ�Ѵ�. 
        // 1. ���� ������ ���Ⱑ �ְ� �� ���� �÷��̰� ������ ���� ���� ���⸦ �ȵ�� �ִٸ� �ݴ´�.
        // 2. ���� ������ ���Ⱑ �ְ� ���� ���� ���⸦ ��� �ִٸ� ���� �������� �ݰ�, ���� ��� �ִ°� ������.
        // 3. ���� ������ ���Ⱑ ���� ���� ���� ���⸦ ��� �ִٸ� ������.

        //3��
        if(_currentWeapon != null)
        {
            DropWeapon(_currentWeapon);
            //���� ����� nulló���ؼ� �����гη� �����
            _weaponList[_currentWeaponIndex] = null;
        }

        //���ٴڿ� ������ �����ִٸ�
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
        //������� ���� ���������� ����Ȱ��̴� Update
        UpdateWeaponUI?.Invoke(_weaponList, _currentWeaponIndex);
    }

    public void DropWeapon(Weapon weapon)
    {
        _weapon = null;
        _currentWeapon = null;
        weapon.StopShooting(); //���� , �̰� �Ʒ��� �����ٲٸ� �ȵȴ�.
        weapon.transform.parent = null;

        //�ѱ��������� ������.
        Vector3 targetPosition = weapon.GetRightDirection() * 0.5f + weapon.transform.position;
        weapon.transform.rotation = Quaternion.identity;
        weapon.transform.localScale = new Vector3(1, 1, 1);

        //�̺κ��� ���� �������� ��ü�ȴ�.
        Sequence seq = DOTween.Sequence();
        seq.Append(weapon.transform.DOMove(targetPosition, 0.5f));
        seq.AppendCallback(() =>
        {
            weapon.DropWeapon.IsActive = true;
        });
        
    }
}

