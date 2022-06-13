using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class BossHealthBar : MonoBehaviour
{
    private Image _healthBar;
    private void Awake()
    {
        _healthBar = transform.Find("BarBackground/Fill").GetComponent<Image>();
    }

    public void SetHealthBar(float normalizedValue)
    {
        _healthBar.fillAmount = normalizedValue;
    }

    //최초 시작시 애니메이션되면서 체력바 채워주도록
    public void InitHealthBar(UnityEvent<float> OnDamaged)
    {
        DOTween.To(() => _healthBar.fillAmount, value => _healthBar.fillAmount = value, 1f, 1f);

        OnDamaged.AddListener(SetHealthBar);
    }

    public void RemoveListener(UnityEvent<float> OnDamaged)
    {
        OnDamaged.RemoveListener(SetHealthBar);
    }
}

