using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackStunAnimation : Feedback
{
    [SerializeField]
    private float _framePerTime = 12; //초당 12장씩
    private SpriteRenderer _spriteRenderer;
    [SerializeField]
    private Sprite[] _sprites;

    private void Awake()
    {
        _spriteRenderer = transform.parent.Find("VisualIcon").GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = false;
    }

    public override void CompletePrevFeedback()
    {
        StopAllCoroutines();
        _spriteRenderer.enabled = false;
    }

    public override void CreateFeedback()
    {
        StartCoroutine(StunAnimationCoroutine());
        _spriteRenderer.enabled = true;
    }

    IEnumerator StunAnimationCoroutine()
    {
        WaitForSeconds ws = new WaitForSeconds(1f / _framePerTime);
        int idx = 0;
        while(true)
        {
            yield return ws;
            _spriteRenderer.sprite = _sprites[idx];
            idx = (idx + 1) % _sprites.Length;
        }
    }
}
