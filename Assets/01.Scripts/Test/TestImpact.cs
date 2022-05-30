using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestImpact : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            for(int i = 0; i < 6000; i++)
            {
                ImpactScript impact = PoolManager.Instance.Pop("ImpactEnemy") as ImpactScript;
                Vector3 pos = Random.insideUnitCircle * 5f;
                impact.SetPositionAndRotation(pos, Quaternion.identity);
            }
        }

        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    for (int i = 0; i < 6000; i++)
        //    {
        //        Vector3 pos = Random.insideUnitCircle * 5f;
        //        TextureParticleManager.Instance.SpawnImpact(pos, 3, 0.6f);
                
        //        //impact.SetPositionAndRotation(pos, Quaternion.identity);
        //    }
        //}
    }
}
