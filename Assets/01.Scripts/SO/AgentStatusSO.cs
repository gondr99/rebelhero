using UnityEngine;

[CreateAssetMenu(menuName = "SO/Agent/Status")]
public class AgentStatusSO : ScriptableObject
{
    //캐릭터의 치명확률
    [Range(0, 0.9f)] public float critical;
    //크리티컬 데미지 비율
    [Range(1.5f, 3f)] public float criticalMinDamage = 1.5f;
    [Range(1.5f, 3f)] public float criticalMaxDamage = 2.5f;

    //캐릭터의 회피 확률
    [Range(0, 0.7f)] public float dodge;

    //최대 체력
    [Range(3, 8)] public int maxHP;

    //최대로 들 수 있는 총의 갯수
    public int maxWeapon = 3;
    
}
