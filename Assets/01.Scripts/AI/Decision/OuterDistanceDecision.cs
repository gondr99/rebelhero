using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OuterDistanceDecision : AIDecision
{
    [field: SerializeField]
    [field: Range(0.1f, 15f)]
    public float distance { get; set; } = 5f;

    public override bool MakeADecision()
    {
        if (_enemyBrain.basePosition != null)
        {
            return Vector3.Distance(_enemyBrain.target.transform.position, _enemyBrain.basePosition.position) > distance;
        }
        return Vector3.Distance(_enemyBrain.target.transform.position, transform.position) > distance;
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
