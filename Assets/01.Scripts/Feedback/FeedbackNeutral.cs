using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackNeutral : Feedback
{
    private SpriteRenderer _spriteRenderer = null;
    [SerializeField]
    private Material _neutralMat = null;
    private Shader _originalMaterialShader;

    private void Awake()
    {
        _spriteRenderer = transform.parent.Find("VisualSprite").GetComponent<SpriteRenderer>();
        _originalMaterialShader = _spriteRenderer.material.shader; //오리지날 셰이더 저장
    }

    public override void CompletePrevFeedback()
    {
        StopAllCoroutines();
        _spriteRenderer.material.SetInt("_MakeNeutralColor", 0);
        _spriteRenderer.material.shader = _originalMaterialShader;
    }
    

    public override void CreateFeedback()
    {
        if (_spriteRenderer.material.HasProperty("_MakeNeutralColor") == false)
        {
            _spriteRenderer.material.shader = _neutralMat.shader;
        }
        
        _spriteRenderer.material.SetInt("_MakeNeutralColor", 1);
    }
}
