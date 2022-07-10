using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class BossHealthBar : MonoBehaviour
{
    private Image _healthBar = null;
    private Image HealthBar
    {
        get
        {
            if(_healthBar == null)
            {
                _healthBar = transform.Find("BarBackground/Fill").GetComponent<Image>();
            }
            return _healthBar;
        }
    }

    public void SetHealthBar(float normalizedValue)
    {
        HealthBar.fillAmount = normalizedValue;
    }

    //최초 시작시 애니메이션되면서 체력바 채워주도록
    public void InitHealthBar(UnityEvent<float> OnDamaged)
    {
        DOTween.To(() => HealthBar.fillAmount, value => HealthBar.fillAmount = value, 1f, 1f);

        OnDamaged.AddListener(SetHealthBar);
    }

    public void RemoveListener(UnityEvent<float> OnDamaged)
    {
        OnDamaged.RemoveListener(SetHealthBar);
    }
}

