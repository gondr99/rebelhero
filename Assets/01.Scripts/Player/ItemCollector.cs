using static Define;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private int _resourceLayer;
    private Player _player;
    
    private void Awake()
    {
        _player = GetComponent<Player>();
        _resourceLayer = LayerMask.NameToLayer("Resource");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == _resourceLayer)
        {
            Resource resource = collision.gameObject.GetComponent<Resource>();

            if (resource != null)
            {
                switch (resource.ResourceData.ResourceType)
                {
                    case ResourceTypeEnum.Coin:
                        GameManager.Instance.Coin += resource.ResourceData.GetAmount();
                        PopupText(resource);
                        resource.PickUpResource();
                        break;
                    case ResourceTypeEnum.Health:
                        _player.Health += resource.ResourceData.GetAmount();
                        PopupText(resource);
                        resource.PickUpResource();
                        break;
                    case ResourceTypeEnum.Ammo:
                        _player.PlayerWeapon.TotalAmmo += resource.ResourceData.GetAmount();
                        PopupText(resource);
                        resource.PickUpResource();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void PopupText(Resource res)
    {
        ResourceDataSO data = res.ResourceData; 
        //숫자 띄워주자. 이거 여기서 하면 안돼.
        PopupText popupText = PoolManager.Instance.Pop("PopupText") as PopupText;
        popupText?.Setup(data.GetAmount(), res.transform.position + new Vector3(0, 0.5f, 0), false, data.popupTextColor);
    }
}
