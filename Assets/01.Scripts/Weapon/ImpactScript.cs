using UnityEngine;

public class ImpactScript : PoolableMono
{
    private AudioSource _audioSource;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        ChildAwake();
    }

    protected virtual void ChildAwake()
    {

    }

    public void DestroyAfterAnimation()
    {
        PoolManager.Instance.Push(this);
    }

    public override void Reset()
    {
        transform.localRotation = Quaternion.identity;
    }

    public virtual void SetPositionAndRotation(Vector3 pos, Quaternion rot)
    {
        transform.SetPositionAndRotation(pos, rot);

        if (_audioSource != null && _audioSource.clip != null)
        {
            //_audioSource.Play();
        }
    }
}
