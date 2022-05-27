using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState : MonoBehaviour
{
    private EnemyAIBrain _enemyBrain = null;
    [SerializeField]
    private List<AIAction> _actions = null;
    [SerializeField]
    private List<AITransition> _transitions = null;

    private void Awake()
    {
        _enemyBrain = transform.GetComponentInParent<EnemyAIBrain>();
    }

    public void UpdateState()
    {
        //수행할 액션들을 모두 수행
        foreach (AIAction action in _actions)
        {
            action.TakeAction();
        }

        //전이가 이뤄져야하는지를 검사
        foreach (AITransition transition in _transitions)
        {
            // 플레이어가 사거리에 있는가? => true 

            //해당 트랜지션에 존재하는 모든 결정들을 결정하게 한다.
            bool result = false;
            foreach (AIDecision decsion in transition.decisions)
            {
                result = decsion.MakeADecision();
                if (!result) break;
            }

            if (result)
            {
                //전이를 할 곳이 있다면 전이해라
                if (transition.positiveResult != null)
                {
                    _enemyBrain.ChangeToState(transition.positiveResult);
                    return;
                }
            }
            else
            {
                //전이 실패
                if (transition.negativeResult != null)
                {
                    _enemyBrain.ChangeToState(transition.negativeResult);
                    return;
                }
            }
        }
    }
}
