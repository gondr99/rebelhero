using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureParticleManager : MonoBehaviour
{
    public static TextureParticleManager Instance;
    private MeshParticleSystem _meshParticleSystem;
    //private ImpactMeshSystem _impactMeshSystem;

    private List<Particle> _bloodList;
    private List<Particle> _shellList;
    //private List<Impact> _impactList;

    private void Awake()
    {
        _meshParticleSystem = GetComponent<MeshParticleSystem>();
        //_impactMeshSystem = GameObject.Find("ImpactMesh").GetComponent<ImpactMeshSystem>();
        Instance = this;
        _bloodList = new List<Particle>();
        _shellList = new List<Particle>();

        //_impactList = new List<Impact>();
    }

    private void Update()
    {
        for (int i = 0; i < _bloodList.Count; i++)
        {
            Particle p = _bloodList[i];
            p.Update();
            if (p.IsComplete())
            {
                _bloodList.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < _shellList.Count; i++)
        {
            Particle p = _shellList[i];
            p.Update();
            if (p.IsComplete())
            {
                _shellList.RemoveAt(i);
                //_meshParticleSystem.DestroyQuad(p.QuadIndex); 
                // 바닥에 떨어져 이동이 끝난 탄피는 제거하고 싶다면 위와 같이 코드를 작성
                i--;
            }
        }

        //for (int i = 0; i < _impactList.Count; i++)
        //{
        //    Impact impact = _impactList[i];
        //    impact.Update();
        //    if (impact.IsComplete == true)
        //    {
        //        _impactList.RemoveAt(i);
        //        i--;
        //    }
        //}
    }

    //public void SpawnImpact(Vector3 pos, int idx, float time)
    //{
    //    int totalSheet = _impactMeshSystem.GetTotalFrame(idx);
    //    Vector3 quadSize = new Vector3(0.8f, 0.8f);
    //    float rot = Random.Range(0, 359f);
    //    _impactList.Add(new Impact(_impactMeshSystem, pos, rot, quadSize, true, idx, totalSheet, time));
    //}

    public void SpawnShell(Vector3 pos, Vector3 dir)
    {
        int uvIndex = _meshParticleSystem.GetRandomShellIndex();
        float moveSpeed = Random.Range(1.5f, 2.5f);
        Vector3 quadSize = new Vector3(0.15f, 0.15f);
        float slowDownFactor = Random.Range(2f, 2.5f);
        _shellList.Add(new Particle(pos, dir, _meshParticleSystem, uvIndex, moveSpeed, quadSize, slowDownFactor, isRotate: true));
    }

    public void SpawnBlood(Vector3 pos, Vector3 dir)
    {
        int uvIndex = _meshParticleSystem.GetRandomBloodIndex();
        float moveSpeed = Random.Range(0.3f, 0.5f);
        float sizeFactor = Random.Range(0.3f, 0.8f);
        Vector3 quadSize = new Vector3(1f, 1f) * sizeFactor;
        float slowDownFactor = Random.Range(0.8f, 1.5f);
        _bloodList.Add(new Particle(pos, dir, _meshParticleSystem, uvIndex, moveSpeed, quadSize, slowDownFactor));
    }

    public void ClearBloodAndShell()
    {
        _meshParticleSystem.DestroyAllQuad();
    }
}
