using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            for(int i = 0; i< 5000; i++)
                MakeRandomImpact();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            for (int i = 0; i < 5000; i++)
                MakeRandomMeshImpact();
        }
    }

    private void MakeRandomImpact()
    {
        float x = Random.Range(-10f, 10f);
        float y = Random.Range(-5f, 5f);

        ImpactScript i = PoolManager.Instance.Pop("ImpactEnemy") as ImpactScript;
        i.SetPositionAndRotation(new Vector3(x, y), Quaternion.identity);

    }

    private void MakeRandomMeshImpact()
    {
        float x = Random.Range(-10f, 10f);
        float y = Random.Range(-5f, 5f);
        TextureParticleManager.Instance.SpawnImpact(new Vector3(x, y), 1, 0.3f);
    }
}
