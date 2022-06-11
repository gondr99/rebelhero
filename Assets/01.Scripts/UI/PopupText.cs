using UnityEngine;
using TMPro;
using DG.Tweening;

public class PopupText : PoolableMono
{
    private TextMeshPro _textMesh;

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshPro>();
    }

    public void Setup(int damageAmount, Vector3 pos, bool isCritical, Color color)
    {
        transform.position = pos;
        _textMesh.SetText(damageAmount.ToString());
        
        if (isCritical)
        {
            _textMesh.color = Color.red;
            _textMesh.fontSize = 10f;
        }else
        {
            _textMesh.color = color;
        }

        ShowingSequence();
    }

    public void Setup(string text, Vector3 pos, Color color, float fontSize = 10f)
    {
        transform.position = pos;
        _textMesh.SetText(text);
        _textMesh.color = color;
        _textMesh.fontSize = fontSize;

        ShowingSequence();
    }

    private void ShowingSequence()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMoveY(transform.position.y + 0.5f, 1f));
        seq.Join(_textMesh.DOFade(0, 1f));
        seq.AppendCallback(() =>
        {
            PoolManager.Instance.Push(this);
        });
    }

    public override void Reset()
    {
        _textMesh.color = Color.white;
        _textMesh.fontSize = 7f;
    }

}
