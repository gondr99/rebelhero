using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static Define;
using Cinemachine;

public class UIManager
{
    public static UIManager Instance = null;

    private RectTransform _tooltipCanvasTrm = null;
    private MessageTooltip _messageTooltip = null;

    private int _weaponTooltipCnt = 0;
    private int _toolTipSortingOrder = 0;

    private RectTransform _uiCanvasTrm = null;
    private RectTransform _bossHealthBarTrm = null;
    private BossHealthBar _bossHealthBar = null;
    private float _bossHealthAnchorY = -150f;

    public UIManager()
    {
        _tooltipCanvasTrm = GameObject.Find("ToopTipCanvas").GetComponent<RectTransform>();
        _messageTooltip = _tooltipCanvasTrm.Find("MessageTooltip").GetComponent<MessageTooltip>();

        _messageTooltip.CloseImmediatly();

        _uiCanvasTrm = GameObject.Find("UICanvas").GetComponent<RectTransform>();

        _bossHealthBarTrm = _uiCanvasTrm.Find("bottomPanel/BossHPBar").GetComponent<RectTransform>();
        _bossHealthBar = _bossHealthBarTrm.GetComponent<BossHealthBar>();

        _bossHealthBarTrm.anchoredPosition = new Vector2(0, _bossHealthAnchorY);
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

    public void EnteringBossRoom(Boss boss, Vector3 offset)
    {
        //카메라 위치 조절
        CinemachineTransposer transposer = VCam.GetCinemachineComponent<CinemachineTransposer>();
        Vector3 target = transposer.m_FollowOffset + offset;
        DOTween.To(() => transposer.m_FollowOffset, value => transposer.m_FollowOffset = value, target, 1f);

        //보스 체력바 등장
        Sequence seq = DOTween.Sequence();
        _bossHealthBar.SetHealthBar(0);
        seq.Append(_bossHealthBarTrm.DOAnchorPos(Vector2.zero, 0.5f));
        seq.AppendCallback(() =>
        {
            _bossHealthBar.InitHealthBar(boss.OnDamaged);
        });
        
    }
    public void ExitBossRoom()
    {

    }
}
