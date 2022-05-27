using UnityEngine;

[CreateAssetMenu(menuName = "SO/Agent/Status")]
public class AgentStatusSO : ScriptableObject
{
    //ĳ������ ġ��Ȯ��
    [Range(0, 0.9f)] public float critical;
    //ũ��Ƽ�� ������ ����
    [Range(1.5f, 3f)] public float criticalMinDamage = 1.5f;
    [Range(1.5f, 3f)] public float criticalMaxDamage = 2.5f;

    //ĳ������ ȸ�� Ȯ��
    [Range(0, 0.7f)] public float dodge;

    //�ִ� ü��
    [Range(3, 8)] public int maxHP;

    //�ִ�� �� �� �ִ� ���� ����
    public int maxWeapon = 3;
    
}
