using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    public static UIManager Instance = null;

    private RectTransform _tooltipCanvasTrm = null;
    private MessageTooltip _messageTooltip = null;

    private int _weaponTooltipCnt = 0;
    private int _toolTipSortingOrder = 0;

    public UIManager()
    {
        _tooltipCanvasTrm = GameObject.Find("ToopTipCanvas").GetComponent<RectTransform>();
        _messageTooltip = _tooltipCanvasTrm.Find("MessageTooltip").GetComponent<MessageTooltip>();

        _messageTooltip.CloseImmediatly();
    }

    public void OpenMessageTooltip(string msg, float time = 0)
    {
        _messageTooltip.ShowText(msg, time);
    }

    public void CloseMessageTooltip()
    {
        _messageTooltip.CloseText();
    }


    
    public WeaponTooltip OpenWeaponTooltip(WeaponDataSO weaponData, Vector3 worldPos)
    {
        WeaponTooltip tooltip = PoolManager.Instance.Pop("WeaponTooltip") as WeaponTooltip;

        tooltip.SetText(weaponData);
        tooltip.PopupToolTip(worldPos, _toolTipSortingOrder);
        _weaponTooltipCnt++;
        _toolTipSortingOrder++;
        return tooltip;
    }

    public void CloseWeaponTooltip(WeaponTooltip tooltip)
    {
        tooltip?.CloseTooltip();
        _weaponTooltipCnt--;
        if (_weaponTooltipCnt == 0)
            _toolTipSortingOrder = 0;
    }
}
