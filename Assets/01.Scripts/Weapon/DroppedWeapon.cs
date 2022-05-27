using UnityEngine;
using static Define;
public class DroppedWeapon : MonoBehaviour
{
    [SerializeField]
    private bool _isActive = false;
    
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            if (_boxCol == null) _boxCol = GetComponent<BoxCollider2D>();
            _boxCol.enabled = _isActive;
        }
    }

    private BoxCollider2D _boxCol = null;
    private Weapon _weapon;
    public Weapon weapon { get => _weapon; }
    private WeaponTooltip _weaponTooltip = null;

    private void Awake()
    {
        _boxCol = GetComponent<BoxCollider2D>();
        _weapon = GetComponent<Weapon>();
        IsActive = false;
    }

    

    //무기정보 패널 표시하기
    public void ShowInfoPanel()
    {
        UIManager.Instance.OpenMessageTooltip("무기 교체를 원하면 X키를 누르세요");
        _weaponTooltip = UIManager.Instance.OpenWeaponTooltip(weapon.WeaponData, transform.position);
    }
    //무기정보 패널감추기
    public void HideInfoPanel()
    {
        UIManager.Instance.CloseMessageTooltip();
        UIManager.Instance.CloseWeaponTooltip(_weaponTooltip);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsActive) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            ShowInfoPanel();
            Player p = collision.gameObject.GetComponent<Player>();
            if (p != null)
            {
                p.PlayerWeapon.dropWeapon = this;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsActive) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            HideInfoPanel();
            Player p = collision.gameObject.GetComponent<Player>();
            if (p != null && p.PlayerWeapon.dropWeapon == this)
            {
                p.PlayerWeapon.dropWeapon = null;
            }
        }
    }

    public void PickUpWeapon()
    {
        HideInfoPanel();
        IsActive = false;
    }
}
