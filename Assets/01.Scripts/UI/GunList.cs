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

    [Header("초기 위치값")]
    [SerializeField] private Vector2 _initAnchorPos;
    private float _xDelta = 7f;
    private RectTransform _gunPanelTemplate;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _panelList = new List<GunPanel>();
        _gunPanelTemplate = transform.Find("GunPanelTemplate").GetComponent<RectTransform>();
        
        _initAnchorPos = _gunPanelTemplate.anchoredPosition; //초기 앵커 포지션
        _gunPanelTemplate.gameObject.SetActive(false);
        _gunPanelTemplate.SetParent( null );
    }

    public void InitUIPanel(List<Weapon> weaponList, int nowIndex)
    {
        List<Weapon> cloneList = weaponList.ToList(); //리스트 복제
        //nowIndex 맞게 리스트를 정렬
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
            if (i < transform.childCount) //재활용 코드
            {
                panelTrm = transform.GetChild(i).GetComponent<RectTransform>();
            }
            else //신규 생성 코드
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

    //패널리스트의 가장 첫번째가 현재 활성화된 무기이다.
    #region 무기 체인징 UI 닷트윈
    public void ChangeWeaponUI(bool isPrev, Action CallBack = null)
    {

        GunPanel first = _panelList.First(); //Linq를 써서 첫번째와 마지막 가져오기
        GunPanel last = _panelList.Last();
        GunPanel next = _panelList[1];

        Sequence seq = DOTween.Sequence(); //시퀀스 생성

        first.weapon?.OnAmmoChange.RemoveAllListeners();  //첫번째 무기의 리스너 제거하고

        if (isPrev)
        {
            seq.Append(first.RectTrm.DOScale(new Vector3(0.9f, 0.9f, 0.9f), _transitionTime)); //작아지고
            seq.Join(first.RectTrm.DOAnchorPos(_initAnchorPos + new Vector2(7, 0), _transitionTime)); //옆으로 밀려나고
            for (int i = 1; i < _panelList.Count - 1; i++)
            {
                seq.Join(_panelList[i].RectTrm.DOAnchorPos(_initAnchorPos + new Vector2(_xDelta * (i + 1), 0), _transitionTime)); //옆으로 밀려나고
            }
            seq.Join(last.RectTrm.DOScale(Vector3.one, _transitionTime)); //사이즈 키우고
            seq.Join(last.RectTrm.DOAnchorPos(_initAnchorPos + new Vector2(0, 82), _transitionTime)); //위로 올리고    

            //스프라이트 정렬 순서 변경
            seq.AppendCallback(() =>
            {
                last.RectTrm.SetAsLastSibling(); //마지막 자식으로 설정해서 맨위로 나오게 하고
                _panelList.RemoveAt(_panelList.Count - 1); //리스트에서 지우고
                _panelList.Insert(0, last); //맨 앞으로 보내준다.
            });
            seq.Append(last.RectTrm.DOAnchorPos(_initAnchorPos, _transitionTime)); //아래로 내리고
        }
        else
        {
            seq.Append(first.RectTrm.DOScale(new Vector3(0.9f, 0.9f, 0.9f), _transitionTime)); //작아지고
            seq.Join(first.RectTrm.DOAnchorPos(_initAnchorPos + new Vector2(0, 82), _transitionTime)); //위로 올리고    
            seq.Join(next.RectTrm.DOScale(Vector3.one, _transitionTime)); //사이즈 키우고
            seq.Join(next.RectTrm.DOAnchorPos(_initAnchorPos, _transitionTime)); //초기 위치로 보내고

            for (int i = 2; i < _panelList.Count; i++)
            {
                seq.Join(_panelList[i].RectTrm.DOAnchorPos(_initAnchorPos + new Vector2(_xDelta * (i - 1), 0), _transitionTime)); //왼쪽으로 땡겨오고
            }
            //스프라이트 정렬 순서 변경
            seq.AppendCallback(() =>
            {
                first.RectTrm.SetAsFirstSibling(); //첫번째 자식으로 설정해서 맨뒤로 보내고
                _panelList.RemoveAt(0); //리스트에서 지우고
                _panelList.Add(first); //맨 뒤로 보낸다.
            });

            seq.Append(first.RectTrm.DOAnchorPos(_initAnchorPos + new Vector2(_xDelta * (_panelList.Count - 1), 0), _transitionTime)); //아래로 내리고
        }
        
        //전환 끝을 알림
        seq.AppendCallback(() =>
        {
            PlaySound(_changeClip);
            ConnectAmmoTextEvent();
            CallBack?.Invoke();
        });
    }
    #endregion 
}
