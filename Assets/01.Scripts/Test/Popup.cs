using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Popup : MonoBehaviour
{
    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn;

    private CanvasGroup _canvasGroup;
    private RectTransform _rectTrm;
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTrm = GetComponent<RectTransform>();
    }
    void Start()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

        _rectTrm.localScale = Vector3.zero;
        _closeBtn.enabled = false;

        _openBtn.onClick.AddListener(() =>
        {
            _openBtn.enabled = false;
            Sequence seq = DOTween.Sequence();

            //DOTween.To(겟터, 셋터, 최종값, 시간   ); 
            seq.Append(DOTween.To(() => _canvasGroup.alpha, v => _canvasGroup.alpha = v, 1f, 0.5f));

            //seq.Append();
            seq.Join(_rectTrm.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f));
            seq.Append(_rectTrm.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.2f));
            seq.Append(_rectTrm.DOScale(new Vector3(1f, 1f, 1f), 0.2f));
            seq.AppendCallback(() => {
                _closeBtn.enabled = true;
                _canvasGroup.blocksRaycasts = true;
                _canvasGroup.interactable = true;
            });
        });

        _closeBtn.onClick.AddListener(() =>
        {
            //참고로 여기는 내일까지 만들어오면 됩니다. 숙제검사 합니다.

            //팝업창이 닫힌후에는 다시 열림버튼이 활성화되도록
            //seq.AppendCallback(() => _openBtn.enabled = true);
        });
    }

}
