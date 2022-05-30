using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleImpact : ImpactScript
{
    private ParticleSystem[] particles;

    protected override void ChildAwake()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
        //자식에 있는 모든 파티클 시스템 가져오고
    }

    public override void SetPositionAndRotation(Vector3 pos, Quaternion rot)
    {
        base.SetPositionAndRotation(pos, rot);
        StartCoroutine(DisableCoroutine());
    }

    IEnumerator DisableCoroutine()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].Play();
        }
        yield return new WaitForSeconds(2f); //각 파티클당 2초후에
        DestroyAfterAnimation(); //애니메이션 종료후에 파티클 처리
    }
}
