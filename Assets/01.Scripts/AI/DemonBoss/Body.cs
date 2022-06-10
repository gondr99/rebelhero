using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour, IHittable
{
    public bool IsEnemy => true;

    public Vector3 _hitPoint { get; set; }

    public void GetHit(int damage, GameObject damageDealer)
    {
        
    }

    
}
