using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class WeaponTooltip : PoolableMono
{
    private static int DefaultSortingOrder = 20;
    
    private TextMeshPro _atkText; //���ݷ� �ؽ�Ʈ
    private TextMeshPro _ammoText; //źâ�뷮
    private TextMeshPro _delayText; //ź�߻� ������
    private TextMeshPro _consumeText; //�ѹ��� �Ҹ�Ǵ� źȯ��

    private List<SpriteRenderer> _childSprite = new List<SpriteRenderer>();
    private SpriteRenderer _panelSprite;
    private void Awake()
    { 
        _atkText = transform.Find("ATKRow/ValueText").GetComponent<TextMeshPro>();
        _ammoText = transform.Find("AmmoRow/ValueText").GetComponent<TextMeshPro>();
        _delayText = transform.Find("DelayRow/ValueText").GetComponent<TextMeshPro>();
        _consumeText = transform.Find("ConsumeRow/ValueText").GetComponent<TextMeshPro>();

        _panelSprite = GetComponent<SpriteRenderer>();

        GetComponentsInChildren(_childSprite);
        _childSprite.RemoveAt(0); //�θ𲨵� ���� �ȵ�.

    }

    public void SetText(WeaponDataSO weaponData)
    {
        _atkText.SetText(weaponData.damageFactor.ToString());
        _ammoText.SetText(weaponData.ammoCapacity.ToString());
        _delayText.SetText(weaponData.weaponDelay.ToString());
        _consumeText.SetText(weaponData.GetBulletCountToSpawn().ToString());
    }


    public void PopupToolTip(Vector3 worldPos, int sortingOrder)
    {
        transform.localScale = new Vector3(1, 0, 1);
        worldPos.y += 1f;
        transform.position = worldPos;
        SetSortingOrder(sortingOrder);
        Open();
    }

    private void SetSortingOrder(int order)
    {
        _panelSprite.sortingOrder = DefaultSortingOrder + order;
        _childSprite.ForEach(x => x.sortingOrder = DefaultSortingOrder + 1 + order);
        _atkText.sortingOrder = DefaultSortingOrder + 1 + order;
        _ammoText.sortingOrder = DefaultSortingOrder + 1 + order;
        _delayText.sortingOrder = DefaultSortingOrder + 1 + order;
        _consumeText.sortingOrder = DefaultSortingOrder + 1 + order;
    }

    private void Open()
    {        
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScaleY(1.2f, 0.3f));
        seq.Append(transform.DOScaleY(0.9f, 0.2f));
        seq.Append(transform.DOScaleY(1f, 0.1f));
    }

    public void CloseTooltip()
    {
        Close();
    }

    private void Close()
    {
        DOTween.Kill(transform);
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScaleY(1.2f, 0.1f));
        seq.Append(transform.DOScaleY(0f, 0.3f));
        seq.AppendCallback(() =>
        {
            PoolManager.Instance.Push(this);
        });
    }

    public override void Reset()
    {
        transform.localScale = new Vector3(1, 0, 1);
        SetSortingOrder(0);
    }
}
