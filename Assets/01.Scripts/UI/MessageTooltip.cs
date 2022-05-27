using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class MessageTooltip : MonoBehaviour
{
    private TextMeshProUGUI _msgText;
    private Sequence _seq = null;

    private int _openCount = 0;

    private void Awake()
    {
        _msgText = transform.Find("MessageText").GetComponent<TextMeshProUGUI>();
    }
    public void ShowText(string msg, float time = 0)
    {
        _openCount++;
        if(_openCount > 1) //기존에 창이 열려있다면
        {
            _msgText.SetText(msg);
            StopAllCoroutines();
            if (time > 0)
            {
                _openCount = 1;
                StartCoroutine(CloseCoroutine(time));
            }
            return;
        }
        _msgText.SetText(msg);

        DOTween.Kill(transform);

        if (_seq != null) _seq.Kill();

        transform.localScale = Vector3.zero;
        _seq = DOTween.Sequence();
        _seq.Append(transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.3f));
        _seq.Append(transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.1f));
        _seq.Append(transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));

        if (time > 0)
        {
            StartCoroutine(CloseCoroutine(time + 0.5f)); 
        }
    }

    IEnumerator CloseCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        CloseText();
    }

    public void CloseText()
    {
        _openCount--;
        if (_openCount > 0) return;

        if (_seq != null) _seq.Kill();
        _seq = DOTween.Sequence();
        _seq.Append(transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f));
        _seq.Append(transform.DOScale(new Vector3(0f, 0f, 0f), 0.3f));
    }

    public void CloseImmediatly()
    {
        if (_seq != null) _seq.Kill();
        transform.localScale = Vector3.zero;
    }
}
