using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleImpact : ImpactScript
{
    private ParticleSystem[] particles;

    protected override void ChildAwake()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
        //�ڽĿ� �ִ� ��� ��ƼŬ �ý��� ��������
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
        yield return new WaitForSeconds(2f); //�� ��ƼŬ�� 2���Ŀ�
        DestroyAfterAnimation(); //�ִϸ��̼� �����Ŀ� ��ƼŬ ó��
    }
}
