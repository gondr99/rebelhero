using UnityEngine;

public class InnerDistanceDecision : AIDecision
{
    [field: SerializeField]
    [field: Range(0.1f, 30f)]
    public float distance { get; set; } = 5f;

    public override bool MakeADecision()
    {
        float calc = 0f;
        //���� �̰��� EnemyBrain���� Base �������� �����ͼ� �װ� ������� �����Ѵ�.
        calc = Vector3.Distance(_enemyBrain.target.transform.position, transform.position);
       
        if (calc < distance)
        {
            if (_aIActionData.targetSpotted == false)
            {
                //���� �þ߾ȿ� ���� �Ѵٸ�
                _aIActionData.targetSpotted = true;
            }
        }
        else
        {
            //���� �þ߾ȿ��� ���ٸ�
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
