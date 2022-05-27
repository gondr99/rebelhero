using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;

public class GunList : MonoBehaviour
{
    private List<GunPanel> _panelList;
    
    [SerializeField] private AudioClip _changeClip;
    [SerializeField] private float _transitionTime = 0.2f;

    private AudioSource _audioSource;

    [Header("�ʱ� ��ġ��")]
    [SerializeField] private Vector2 _initAnchorPos;
    private float _xDelta = 7f;
    private RectTransform _gunPanelTemplate;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _panelList = new List<GunPanel>();
        _gunPanelTemplate = transform.Find("GunPanelTemplate").GetComponent<RectTransform>();
        
        _initAnchorPos = _gunPanelTemplate.anchoredPosition; //�ʱ� ��Ŀ ������
        _gunPanelTemplate.gameObject.SetActive(false);
        _gunPanelTemplate.SetParent( null );
    }

    public void InitUIPanel(List<Weapon> weaponList, int nowIndex)
    {
        List<Weapon> cloneList = weaponList.ToList(); //����Ʈ ����
        //nowIndex �°� ����Ʈ�� ����
        for(int i = 0; i < nowIndex; i++)
        {
            Weapon first = cloneList.First();
            cloneList.Remove(first);
            cloneList.Add(first);
        }

        cloneList.Reverse();
        _panelList.Clear();

        
        for (int i = 0; i < cloneList.Count; i++)
        {
            RectTransform panelTrm = null;
            if (i < transform.childCount) //��Ȱ�� �ڵ�
            {
                panelTrm = transform.GetChild(i).GetComponent<RectTransform>();
            }
            else //�ű� ���� �ڵ�
            {
                panelTrm = Instantiate(_gunPanelTemplate, transform);
            }
            panelTrm.gameObject.SetActive(true);
            panelTrm.anchoredPosition = _initAnchorPos + new Vector2( (cloneList.Count - i -1 )  * _xDelta, 0);
            if(i != cloneList.Count - 1)
            {
                panelTrm.localScale = new Vector3(0.9f, 0.9f, 1f);
            }

            GunPanel gunPanel = panelTrm.GetComponent<GunPanel>();
            gunPanel.Init(cloneList[i]);
            _panelList.Add(gunPanel);
        }
        _panelList.Reverse();

        ConnectAmmoTextEvent();
    }

    private void PlaySound(AudioClip clip)
    {
        _audioSource.Stop();
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    private void ConnectAmmoTextEvent()
    {
        GunPanel first = _panelList.First();
        first.weapon?.OnAmmoChange.AddListener((amount) =>
        {
            first.UpdateBulletText(amount);
        });
    }

    //�гθ���Ʈ�� ���� ù��°�� ���� Ȱ��ȭ�� �����̴�.
    #region ���� ü��¡ UI ��Ʈ��
    public void ChangeWeaponUI(bool isPrev, Action CallBack = null)
    {

        GunPanel first = _panelList.First(); //Linq�� �Ἥ ù��°�� ������ ��������
        GunPanel last = _panelList.Last();
        GunPanel next = _panelList[1];

        Sequence seq = DOTween.Sequence(); //������ ����

        first.weapon?.OnAmmoChange.RemoveAllListeners();  //ù��° ������ ������ �����ϰ�

        if (isPrev)
        {
            seq.Append(first.RectTrm.DOScale(new Vector3(0.9f, 0.9f, 0.9f), _transitionTime)); //�۾�����
            seq.Join(first.RectTrm.DOAnchorPos(_initAnchorPos + new Vector2(7, 0), _transitionTime)); //������ �з�����
            for (int i = 1; i < _panelList.Count - 1; i++)
            {
                seq.Join(_panelList[i].RectTrm.DOAnchorPos(_initAnchorPos + new Vector2(_xDelta * (i + 1), 0), _transitionTime)); //������ �з�����
            }
            seq.Join(last.RectTrm.DOScale(Vector3.one, _transitionTime)); //������ Ű���
            seq.Join(last.RectTrm.DOAnchorPos(_initAnchorPos + new Vector2(0, 82), _transitionTime)); //���� �ø���    

            //��������Ʈ ���� ���� ����
            seq.AppendCallback(() =>
            {
                last.RectTrm.SetAsLastSibling(); //������ �ڽ����� �����ؼ� ������ ������ �ϰ�
                _panelList.RemoveAt(_panelList.Count - 1); //����Ʈ���� �����
                _panelList.Insert(0, last); //�� ������ �����ش�.
            });
            seq.Append(last.RectTrm.DOAnchorPos(_initAnchorPos, _transitionTime)); //�Ʒ��� ������
        }
        else
        {
            seq.Append(first.RectTrm.DOScale(new Vector3(0.9f, 0.9f, 0.9f), _transitionTime)); //�۾�����
            seq.Join(first.RectTrm.DOAnchorPos(_initAnchorPos + new Vector2(0, 82), _transitionTime)); //���� �ø���    
            seq.Join(next.RectTrm.DOScale(Vector3.one, _transitionTime)); //������ Ű���
            seq.Join(next.RectTrm.DOAnchorPos(_initAnchorPos, _transitionTime)); //�ʱ� ��ġ�� ������

            for (int i = 2; i < _panelList.Count; i++)
            {
                seq.Join(_panelList[i].RectTrm.DOAnchorPos(_initAnchorPos + new Vector2(_xDelta * (i - 1), 0), _transitionTime)); //�������� ���ܿ���
            }
            //��������Ʈ ���� ���� ����
            seq.AppendCallback(() =>
            {
                first.RectTrm.SetAsFirstSibling(); //ù��° �ڽ����� �����ؼ� �ǵڷ� ������
                _panelList.RemoveAt(0); //����Ʈ���� �����
                _panelList.Add(first); //�� �ڷ� ������.
            });

            seq.Append(first.RectTrm.DOAnchorPos(_initAnchorPos + new Vector2(_xDelta * (_panelList.Count - 1), 0), _transitionTime)); //�Ʒ��� ������
        }
        
        //��ȯ ���� �˸�
        seq.AppendCallback(() =>
        {
            PlaySound(_changeClip);
            ConnectAmmoTextEvent();
            CallBack?.Invoke();
        });
    }
    #endregion 
}
