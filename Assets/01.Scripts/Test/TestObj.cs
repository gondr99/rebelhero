using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;  //이걸 넣어야 닷트윈의 확장매서드를 쓸 수 있어

public class TestObj : MonoBehaviour
{
    [SerializeField]
    private float _speed;

    private Vector3 _destination;
    private Camera _mainCam;

    void Start()
    {
        _mainCam = Camera.main;
        _destination = transform.position;
        
    }
    [SerializeField]
    private float _str;
    [SerializeField]
    private int _vibe;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _destination = _mainCam.ScreenToWorldPoint(Input.mousePosition);
            _destination.z = 0;
            MoveToDirection();
            //도착하면 720도 회전한다. 
            // 회전이 끝나면 콘솔창에 끝 이라고 출력한다.
        }

    }

    private void OnMouseEnter()
    {
        //transform.DOKill(); //transform에서 수행되고 있던 모든 트윈을 종료한다.
        //여기에 쉐이킹 전의 원래있던 곳으로 돌려주는 코드도 필요하다.
        //transform.DOShakePosition(0.5f, _str, _vibe);
    }

    [SerializeField]
    private Ease _ease;
    private void MoveToDirection()
    {
        Sequence seq = DOTween.Sequence(); //자동으로 시퀀스 객체를 하나 만들어서 보내줘

        Vector3 dir = _destination - transform.position;
        float time = dir.magnitude / _speed;

        //Append한 녀석들은 순서대로 실행된다.
        //이동이 끝나면
        seq.Append(transform.DOMove(_destination, time));
        //회전을 시작
        seq.Append(transform.DORotate(new Vector3(0, 0, 720f), 0.8f, RotateMode.FastBeyond360));

        seq.AppendCallback(() =>
        {
            Debug.Log(time); //클로져 (closure)
            Debug.Log("트윈 종료");
        });        
    }
}
