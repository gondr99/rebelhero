using UnityEngine;

public interface IRangeWeapon
{
    Vector3 GetRightDirection();
    Vector3 GetFirePoisition();
    Vector3 GetShellEjectPostion();
    Vector3 GetEjectDirection();
}
