using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestScript : MonoBehaviour
{
    //대리자 
    public delegate int Add(int x);

    public delegate int Add2(int a, int b);
    Action<int> a; //int형 파라메터 하나를 받는 void 타입 함수
    UnityEvent<int> c;

    Func<string, int> b; // 파라메터를 스트링 하나를 받고 리턴타입이 int인 함수를 넣을 수 있어

    private void Start()
    {
        
        int a;
        a = 10; // 변수의 도메인 
        // 함수를 변수에 담고 싶다.
        //
        // 1. 인터넷에서 파일을 다운받고   void DownFile(  ShowScreen   ) { ShowScreen(); }
        // ------시간 ----
        // 2. 다운이 완료되면 화면에 띄워라 

        //익명함수 .. 이름이 없는 함수  람다수식
        // 1 . delegate를 제거하고 람다기호 =>
        // 2. 파라메터의 타입은 생략 가능하다
        // 3. 파라메터가 한개라면 소괄호 생략 가능
        // 4. 한줄짜리 코드면 그게리턴이든 아니든 중괄호 생략가능
        Add b = x => x + 4;

        List<GameObject> list = new List<GameObject>();

        list.FindAll(x => x.activeSelf).ForEach(x => x.SetActive(false));
        
    }

    public int Add4(int value)
    {
        //alt + shift + .
        return value + 4;
    }
    public int Add8(int value)
    {
        return value + 8;
    }

}
