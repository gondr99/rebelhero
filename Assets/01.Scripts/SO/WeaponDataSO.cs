using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Weapon/WeaponData")]
public class WeaponDataSO : ScriptableObject
{    
    public GameObject prefab;

    //총알 데이터가 있어야해 

    [Range(0, 999)] public int ammoCapacity = 100; //탄창 크기
    public bool automaticFire; //연사 가능하냐?
    
    [Range(0.1f, 2f)] public float weaponDelay = 0.1f; //사격지연
    [Range(0, 10f)] public float spreadAngle = 5f; //이건 탄이 퍼지는 각도

    [SerializeField] private bool _multiBulletShoot = false; //이거는 한번 클릭에 여러발 발사
    [SerializeField] private int _bulletCount = 1;

    public int damageFactor = 1;

    [Range(0.1f, 2f)] public float reloadTime = 0.1f;

    public AudioClip shootClip;
    public AudioClip outOfAmmoClip;
    public AudioClip reloadClip;

    public Sprite sprite;

    public bool infiniteAmmo = false; //무한 탄창

    public int GetBulletCountToSpawn()
    {
        return _multiBulletShoot ? _bulletCount : 1;
    }
}
