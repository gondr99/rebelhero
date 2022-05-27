using UnityEngine;
using UnityEngine.Events;

public class LookDecision : AIDecision
{
    [field: SerializeField]
    [field: Range(0.1f, 15f)]
    public float distance { get; set; } = 5f;

    [field: SerializeField]
    public UnityEvent OnPlayerSpotted { get; set; }

    public LayerMask raycastMask;

    public override bool MakeADecision()
    {
        Vector3 dir = _enemyBrain.target.transform.position - transform.position;
        dir.z = 0;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, distance, raycastMask);


        if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            OnPlayerSpotted?.Invoke();
            return true;
        }
        //else
        //{
        //    Debug.Log(hit.collider.gameObject.name);
        //}
        return false;
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.Selection.activeGameObject == gameObject && _enemyBrain != null && _enemyBrain.target != null)
        {
            Gizmos.color = Color.red;
            Vector3 dir = _enemyBrain.target.transform.position - transform.position;
            dir.z = 0;
            Gizmos.DrawRay(transform.position, dir.normalized * distance);
            Gizmos.color = Color.white;
        }
    }
#endif
}
