using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour, IRangeWeapon
{
    #region �� ������
    [SerializeField] protected WeaponDataSO _weaponData;
    [SerializeField] protected GameObject _muzzle; //�ѱ��� ��ġ
    [SerializeField] protected Transform _shellEjectPos; //ź�� ���� ����

    [SerializeField] protected bool _isEnemyWeapon = false;
    #endregion

    public WeaponDataSO WeaponData { get => _weaponData; }

    #region Ammo���� �ڵ�
    public UnityEvent<int> OnAmmoChange; 
    [SerializeField] protected int _ammo; //���� �Ѿ� ��
    public int Ammo
    {
        get
        {
            return _weaponData.infiniteAmmo ? -1 : _ammo; //���� źȯ���� ����ź���̸� -1�ƴϸ� ���� �� ����
        }
        set
        {
            _ammo = Mathf.Clamp(value, 0, _weaponData.ammoCapacity);
            OnAmmoChange?.Invoke(_ammo);
        }
    }
    public bool AmmoFull { get => Ammo == _weaponData.ammoCapacity || _weaponData.infiniteAmmo; }
    public int EmptyBulletCnt { get => _weaponData.ammoCapacity - _ammo; } //������ źȯ�� ��ȯ
    #endregion

    #region �߻����
    public UnityEvent OnShoot;
    public UnityEvent OnShootNoAmmo;
    public UnityEvent OnStopShooting; //�߻����� �ڵ�
    protected bool _isShooting = false;
    [SerializeField] protected bool _delayCoroutine = false;
    #endregion

    #region �����������
    private DroppedWeapon _dropWeapon;
    public DroppedWeapon DropWeapon { get => _dropWeapon; }
    #endregion


    #region ���� ������ ����
    private WeaponRenderer _weaponRenderer;
    public WeaponRenderer Render
    {
        get => _weaponRenderer;
    }
    #endregion

    private void Awake()
    {
        _ammo = _weaponData.ammoCapacity;
        WeaponAudio audio = transform.Find("WeaponAudio").GetComponent<WeaponAudio>();
        audio.SetAudioClip(_weaponData.shootClip, _weaponData.outOfAmmoClip, _weaponData.reloadClip);
        _dropWeapon = GetComponent<DroppedWeapon>();
        _dropWeapon.IsActive = false;

        _weaponRenderer = GetComponent<WeaponRenderer>();
    }

    public void ResetWeapon()
    {
        _isShooting = false;
        _delayCoroutine = false;
    }

    private void Update()
    {
        UseWeapon();
    }

    private void UseWeapon()
    {
        //���� ������̰� ���������� �ƴ϶��
        if(_isShooting && _delayCoroutine == false)
        {
            if(Ammo > 0 || _weaponData.infiniteAmmo)
            {
                if(!_weaponData.infiniteAmmo)
                {
                    Ammo -= _weaponData.GetBulletCountToSpawn(); //���� ź�� ���� �ƴϸ� ź�� ����
                }

                OnShoot?.Invoke(); //���� ����
                for(int i = 0; i < _weaponData.GetBulletCountToSpawn(); i++)
                {
                    ShootBullet(); //���� �����մϴ�. �Ƹ��� ������?
                }
            }
            else
            {
                _isShooting = false;
                OnShootNoAmmo?.Invoke();
                return;
            }
            FinishShooting(); //�ѹ� ����� ���������� ���´ٸ� ����� ����
        }
    }

    protected void FinishShooting()
    {
        StartCoroutine(DelayNextShootCoroutine());
        if(_weaponData.automaticFire == false)
        {
            _isShooting = false;
        }
    }

    protected IEnumerator DelayNextShootCoroutine()
    {
        _delayCoroutine = true;
        yield return new WaitForSeconds(_weaponData.weaponDelay);
        _delayCoroutine = false;
    }

    private void ShootBullet()
    {
        SpawnBullet(_muzzle.transform.position, CalculateAngle(_muzzle), _isEnemyWeapon);
    }

    private Quaternion CalculateAngle(GameObject muzzle)
    {
        float spread = Random.Range(-_weaponData.spreadAngle, _weaponData.spreadAngle);
        Quaternion bulletSpreadRot = Quaternion.Euler(new Vector3(0, 0, spread));
        return muzzle.transform.rotation * bulletSpreadRot; //���� ȸ������ ����ȸ�� �߰�
    }

    private void SpawnBullet(Vector3 position, Quaternion rot, bool isEnemyBullet)
    {
        Bullet bullet = PoolManager.Instance.Pop("Bullet") as Bullet;
        bullet.SetPositionAndRotation(position, rot);
        bullet.IsEnemy = isEnemyBullet;
        bullet.BulletData = _weaponData.bulletData;
        bullet.damageFactor = _weaponData.damageFactor; //������ ������ ���� �־��ش�.
    }

    //��� �����ϴٸ� ��� ����
    public void TryShooting()
    {
        _isShooting = true;
    }

    public void StopShooting()
    {
        _isShooting = false;
        OnStopShooting?.Invoke();
    }

    public Vector3 GetRightDirection()
    {
        return transform.right;
    }

    public Vector3 GetFirePoisition()
    {
        return _muzzle.transform.position;
    }

    public Vector3 GetShellEjectPostion()
    {
        return _shellEjectPos.position;
    }

    public Vector3 GetEjectDirection()
    {
        if(transform.localScale.y < 0)
        {
            //��������
            return (transform.right * -0.5f + transform.up).normalized;
        }
        else
        {
            //����������
            return (transform.right * 0.5f + transform.up).normalized * -1;
        }
    }
}
