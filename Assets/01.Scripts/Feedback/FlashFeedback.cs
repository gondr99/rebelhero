using System.Collections;
using UnityEngine;

public class FlashFeedback : Feedback
{
    [SerializeField]
    private SpriteRenderer _spriteRenderer = null;
    [SerializeField]
    private float _flashTime = 0.1f;
    [SerializeField]
    private Material _flashMat = null;

    private Shader _originalMaterialShader;

    private void Awake()
    {
        if(_spriteRenderer == null)
            _spriteRenderer = transform.parent.Find("VisualSprite").GetComponent<SpriteRenderer>();
        _originalMaterialShader = _spriteRenderer.material.shader; //�������� ���̴� ����
    }

    public override void CompletePrevFeedback()
    {
        StopAllCoroutines();
        _spriteRenderer.material.SetInt("_MakeSolidColor", 0);
        _spriteRenderer.material.shader = _originalMaterialShader;
    }

    public override void CreateFeedback()
    {
        if (_spriteRenderer.material.HasProperty("_MakeSolidColor") == false)
        {
            _spriteRenderer.material.shader = _flashMat.shader;

        }
        //boolean�� 1, 0�� true falseǥ��
        _spriteRenderer.material.SetInt("_MakeSolidColor", 1);
        StartCoroutine(WaitBeforeChangingBack());
    }

    IEnumerator WaitBeforeChangingBack()
    {
        yield return new WaitForSeconds(_flashTime);
        if (_spriteRenderer.material.HasProperty("_MakeSolidColor"))
        {
            //boolean�� 1, 0�� true falseǥ��
            _spriteRenderer.material.SetInt("_MakeSolidColor", 0);
        }
        else
        {
            _spriteRenderer.material.shader = _originalMaterialShader;
        }
    }
}
