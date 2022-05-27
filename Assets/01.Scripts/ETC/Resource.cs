using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class Resource : PoolableMono
{
    [field: SerializeField]
    public ResourceDataSO ResourceData { get; set; }

    private AudioSource _audioSoruce;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _audioSoruce = GetComponent<AudioSource>();
        _audioSoruce.clip = ResourceData.useSound;
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PickUpResource()
    {
        //숫자 띄워주자.
        PopupText popupText = PoolManager.Instance.Pop("PopupText") as PopupText;
        popupText?.Setup(ResourceData.GetAmount(), transform.position + new Vector3(0, 0.5f, 0), false, ResourceData.popupTextColor);
        StartCoroutine(DestroyCoroutine());
    }

    IEnumerator DestroyCoroutine()
    {
        _collider.enabled = false;
        _spriteRenderer.enabled = false;
        _audioSoruce.Play();
        yield return new WaitForSeconds(_audioSoruce.clip.length + 0.3f);

        PoolManager.Instance.Push(this);
    }

    private void OnDisable()
    {
        StopAllCoroutines(); //현재 돌고 있는 코루틴 전부 종료
        transform.DOKill(); //구동중인 트윈도 종료
    }

    public override void Reset()
    {
        _spriteRenderer.enabled = true;
        _collider.enabled = true;
    }
}
