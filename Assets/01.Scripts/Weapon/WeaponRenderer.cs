using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRenderer : MonoBehaviour
{
    [SerializeField] protected int _playerSortingOrder = 0;
    protected SpriteRenderer _weaponRenderer;

    private void Awake()
    {
        _weaponRenderer = GetComponent<SpriteRenderer>();
    }

    public void FlipSprite(bool value)
    {
        Vector3 localScale = new Vector3(1, 1, 1);
        if (value)
            localScale.y = -1;

        transform.localScale = localScale;
    }

    public void RenderBehindHead(bool value)
    {
        if(value)
        {
            _weaponRenderer.sortingOrder = _playerSortingOrder - 1;
        }else
        {
            _weaponRenderer.sortingOrder = _playerSortingOrder + 1;

        }
    }
}
