using UnityEngine;
[CreateAssetMenu(menuName = "SO/Enemies/EnemyData")]
public class EnemyDataSO : ScriptableObject
{
    public string enemyName;
    public GameObject prefab;
    public PoolableMono poolPrefab;
    [field: SerializeField]
    public int maxHealth { get; set; } = 3;

    public float knockRegist = 1f; //넉백파워 저항력

    //공격관련 데이터
    [field: SerializeField]
    public int damage { get; set; } = 1;
    public float attackDelay = 1;
    public float hitRange = 0;
}
