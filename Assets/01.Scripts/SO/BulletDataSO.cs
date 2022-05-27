using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Weapon/BulletData")]
public class BulletDataSO : ScriptableObject
{
    public GameObject prefab;
    [Range(1, 10)] public int damage = 1;
    [Range(1, 100)] public float bulletSpeed = 1;
    [Range(0, 100f)] public float friction = 0;
    [Range(0, 5f)] public float explosionRadius = 3f;

    public Sprite sprite;
    public RuntimeAnimatorController animatorController;

    public bool bounce = false; //튕기는지 여부
    public bool goThroughHit = false; //관통

    public bool isRayCast = false; //레이저 캐스팅인지

    public GameObject impactObstaclePrefab;
    public GameObject impactEnemyPrefab;

    [Range(1, 20)] public float knockBackPower = 5;
    [Range(0.01f, 1f)] public float knockBackDelay = 0.1f;

    public Material bulletMaterial;
    public bool isCharging = false;

    public float lifeTime = 2f;
}
