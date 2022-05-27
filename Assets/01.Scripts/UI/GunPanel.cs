using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunPanel : MonoBehaviour
{
    [SerializeField] private Weapon _weapon;

    private Image _weaponImage;
    private TextMeshProUGUI _ammoText;

    private RectTransform _rectTrm = null;
    private float _initFontSize;
    private Color _transparentColor = new Color(0, 0, 0, 0);

    public RectTransform RectTrm
    {
        get
        {
            if (_rectTrm == null)
            {
                _rectTrm = GetComponent<RectTransform>();
            }
            return _rectTrm;
        }
    }
    public Weapon weapon
    {
        get => _weapon;
    }

    private void Awake()
    {
        _weaponImage = transform.Find("GunImage").GetComponent<Image>();
        _ammoText = transform.Find("RemainBullet").GetComponent<TextMeshProUGUI>();
        _rectTrm = GetComponent<RectTransform>();
    }

    public void Init(Weapon weapon)
    {
        _weapon = weapon;
        _initFontSize = _ammoText.fontSize;
        if (_weapon != null)
        {
            _weaponImage.sprite = _weapon.WeaponData.sprite;
            _weaponImage.color = Color.white;
            UpdateBulletText(_weapon.Ammo);
        }
        else
        {
            _weaponImage.sprite = null;
            _weaponImage.color = _transparentColor;
            UpdateBulletText(0);
        }
    }

    public void UpdateBulletText(int amount)
    {
        if (amount < 0)
        {
            //무한 탄창일 경우
            _ammoText.color = Color.blue;
            _ammoText.fontSize = _initFontSize + 8;
            _ammoText.SetText("∞");
            return;
        }
        _ammoText.fontSize = _initFontSize;
        if (amount == 0)
        {
            _ammoText.color = Color.red;
        }
        else
        {
            _ammoText.color = Color.white;
        }
        _ammoText.SetText(amount.ToString());
    }
}
