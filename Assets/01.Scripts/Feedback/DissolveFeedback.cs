using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class DissolveFeedback : Feedback
{     
    private SpriteRenderer _spriterRenderer = null;
    [SerializeField]
    private float _duration = 0.05f;
    [field: SerializeField]
    public UnityEvent DeathCallback { get; set; }


    private void Awake()
    {
        _spriterRenderer = transform.parent.Find("VisualSprite").GetComponent<SpriteRenderer>();
    }

    public override void CompletePrevFeedback()
    {
        _spriterRenderer.DOComplete();
        _spriterRenderer.material.DOComplete();
        _spriterRenderer.material.SetFloat("_Dissolve", 1);
    }

    public override void CreateFeedback()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(_spriterRenderer.material.DOFloat(0, "_Dissolve", _duration));
        if (DeathCallback != null)
        {
            seq.AppendCallback(() => DeathCallback.Invoke());
        }
    }
}
