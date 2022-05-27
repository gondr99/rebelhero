using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour, IRangeWeapon
{
    #region 총 데이터
    [SerializeField] protected WeaponDataSO _weaponData;
    [SerializeField] protected GameObject _muzzle; //총구의 위치
    [SerializeField] protected Transform _shellEjectPos; //탄피 생성 지점

    [SerializeField] protected bool _isEnemyWeapon = false;
    #endregion

    public WeaponDataSO WeaponData { get => _weaponData; }

    #region Ammo관련 코드
    public UnityEvent<int> OnAmmoChange; 
    [SerializeField] protected int _ammo; //현재 총알 수
    public int Ammo
    {
        get
        {
            return _weaponData.infiniteAmmo ? -1 : _ammo; //남은 탄환수를 무한탄약이면 -1아니면 남은 수 리턴
        }
        set
        {
            _ammo = Mathf.Clamp(value, 0, _weaponData.ammoCapacity);
            OnAmmoChange?.Invoke(_ammo);
        }
    }
    public bool AmmoFull { get => Ammo == _weaponData.ammoCapacity || _weaponData.infiniteAmmo; }
    public int EmptyBulletCnt { get => _weaponData.ammoCapacity - _ammo; } //부족한 탄환수 반환
    #endregion

    #region 발사로직
    public UnityEvent OnShoot;
    public UnityEvent OnShootNoAmmo;
    public UnityEvent OnStopShooting; //발사종료 코드
    protected bool _isShooting = false;
    [SerializeField] protected bool _delayCoroutine = false;
    #endregion

    #region 웨폰드랍관련
    private DroppedWeapon _dropWeapon;
    public DroppedWeapon DropWeapon { get => _dropWeapon; }
    #endregion


    #region 웨폰 렌더러 관련
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
        //현재 사격중이고 재장전중이 아니라면
        if(_isShooting && _delayCoroutine == false)
        {
            if(Ammo > 0 || _weaponData.infiniteAmmo)
            {
                if(!_weaponData.infiniteAmmo)
                {
                    Ammo -= _weaponData.GetBulletCountToSpawn(); //무한 탄약 총이 아니면 탄약 감소
                }

                OnShoot?.Invoke(); //실제 슈팅
                for(int i = 0; i < _weaponData.GetBulletCountToSpawn(); i++)
                {
                    ShootBullet(); //차후 구현합니다. 아마도 다음주?
                }
            }
            else
            {
                _isShooting = false;
                OnShootNoAmmo?.Invoke();
                return;
            }
            FinishShooting(); //한발 사격을 성공적으로 끝냈다면 해줘야 할일
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
        return muzzle.transform.rotation * bulletSpreadRot; //원본 회전율에 떨림회전 추가
    }

    private void SpawnBullet(Vector3 position, Quaternion rot, bool isEnemyBullet)
    {
        Bullet bullet = PoolManager.Instance.Pop("Bullet") as Bullet;
        bullet.SetPositionAndRotation(position, rot);
        bullet.IsEnemy = isEnemyBullet;
        bullet.BulletData = _weaponData.bulletData;
        bullet.damageFactor = _weaponData.damageFactor; //무기의 데미지 곱을 넣어준다.
    }

    //사격 가능하다면 사격 시작
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
            //왼쪽으로
            return (transform.right * -0.5f + transform.up).normalized;
        }
        else
        {
            //오른쪽으로
            return (transform.right * 0.5f + transform.up).normalized * -1;
        }
    }
}
