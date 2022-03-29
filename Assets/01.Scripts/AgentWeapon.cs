using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentWeapon : MonoBehaviour
{
    protected Weapon _weapon;

    protected WeaponRenderer _weaponRenderer;

    protected float _desireAngle; //무기를 바라보는 방향

    private void Awake()
    {
        AssignWeapon();
        AwakeChild();
    }

    protected virtual void AwakeChild()
    {
        //do nothing;
    }

    public virtual void AssignWeapon()
    {
        _weaponRenderer = GetComponentInChildren<WeaponRenderer>();
        _weapon = GetComponentInChildren<Weapon>();
    }

    public virtual void AimWeapon(Vector2 pointerPos)
    {
        if (_weapon == null) return; //가지고 있는 무기가 없으면 리턴

        Vector3 aimDirection = (Vector3)pointerPos - transform.position;
        //360도 각도로 목적지까지의 각도를 계산하고
        _desireAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        //삼각형에서 

        AdjustWeaponRendering();
        
        transform.rotation = Quaternion.AngleAxis(_desireAngle, Vector3.forward); //z축 기준 회전

    }

    private void AdjustWeaponRendering()
    {
        if(_weaponRenderer != null)
        {
            _weaponRenderer.FlipSprite( _desireAngle > 90f || _desireAngle < -90f ); //언제 true야?
            _weaponRenderer.RenderBehindHead( _desireAngle > 0 && _desireAngle < 180 );
        }
    }

    public virtual void Shoot()
    {
        if(_weapon != null)
        {
            _weapon.TryShooting();
        }
    }

    public virtual void StopShooting()
    {
        if(_weapon != null)
        {
            _weapon.StopShooting();
        }
    }
}
