using UnityEngine;

public class InnerDistanceDecision : AIDecision
{
    [field: SerializeField]
    [field: Range(0.1f, 30f)]
    public float distance { get; set; } = 5f;

    public override bool MakeADecision()
    {
        float calc = 0f;
        //차후 이곳에 EnemyBrain에서 Base 포지션을 가져와서 그걸 기반으로 가야한다.
        calc = Vector3.Distance(_enemyBrain.target.transform.position, transform.position);
       
        if (calc < distance)
        {
            if (_aIActionData.targetSpotted == false)
            {
                //적이 시야안에 존재 한다면
                _aIActionData.targetSpotted = true;
            }
        }
        else
        {
            //적이 시야안에는 없다면
            _aIActionData.targetSpotted = false;
        }
        return _aIActionData.targetSpotted;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.Selection.activeObject == gameObject)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, distance);
            Gizmos.color = Color.white;
        }
    }
#endif
}
