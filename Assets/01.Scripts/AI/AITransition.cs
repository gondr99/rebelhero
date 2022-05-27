using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITransition : MonoBehaviour
{
    //트랜지션을 정할 여러개의 조건들을 정의
    [field: SerializeField]
    public List<AIDecision> decisions { get; set; }

    //모든 조건이 만족하면 positive
    [field: SerializeField]
    public AIState positiveResult { get; set; }

    //조건중 하나라도 만족하지 않으면 negative
    [field: SerializeField]
    public AIState negativeResult { get; set; }

}
