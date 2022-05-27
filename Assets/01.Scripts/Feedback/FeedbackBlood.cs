using UnityEngine;

public class FeedbackBlood : Feedback
{
    [SerializeField]
    private float _randomRange = 0.3f;

    private IHittable _agent;
    private void Awake()
    {
        _agent = GetComponentInParent<IHittable>();
    }
    public override void CompletePrevFeedback()
    {
        //do nothing
    }

    public override void CreateFeedback()
    {
        Vector3 pos = transform.position + new Vector3(Random.Range(-_randomRange, +_randomRange), Random.Range(-_randomRange, +_randomRange), 0);

        Vector3 dir = (transform.position - _agent._hitPoint).normalized;
        TextureParticleManager.Instance.SpawnBlood(pos, dir);
    }
}
