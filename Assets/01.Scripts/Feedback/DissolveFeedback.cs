using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class DissolveFeedback : Feedback
{   
    [SerializeField]
    private SpriteRenderer _spriteRenderer = null;
    [SerializeField]
    private float _duration = 0.05f;
    [field: SerializeField]
    public UnityEvent DeathCallback { get; set; }


    private void Awake()
    {
        if(_spriteRenderer == null)
            _spriteRenderer = transform.parent.Find("VisualSprite").GetComponent<SpriteRenderer>();
    }

    public override void CompletePrevFeedback()
    {
        _spriteRenderer.DOComplete();
        _spriteRenderer.material.DOComplete();
        _spriteRenderer.material.SetFloat("_Dissolve", 1);
    }

    public override void CreateFeedback()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(_spriteRenderer.material.DOFloat(0, "_Dissolve", _duration));
        if (DeathCallback != null)
        {
            seq.AppendCallback(() => DeathCallback.Invoke());
        }
    }
}
