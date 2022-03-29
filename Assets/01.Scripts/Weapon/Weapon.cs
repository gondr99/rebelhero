using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    #region 총 데이터
    [SerializeField] protected WeaponDataSO _weaponData;
    [SerializeField] protected GameObject _muzzle; //총구의 위치
    [SerializeField] protected Transform _shellEjectPos; //탄피 생성 지점
    #endregion

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

    protected bool _isShooting = false;
    [SerializeField] protected bool _delayCoroutine = false;
    #endregion

    private void Awake()
    {
        _ammo = _weaponData.ammoCapacity;
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
                    Ammo--; //무한 탄약 총이 아니면 탄약 감소
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
        Debug.Log("발사");
    }

    //사격 가능하다면 사격 시작
    public void TryShooting()
    {
        _isShooting = true;
    }

    public void StopShooting()
    {
        _isShooting = false;
    }
}
