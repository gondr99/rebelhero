using UnityEngine;

[CreateAssetMenu(menuName = "SO/Trap/TrapData")]
public class TrapDataSO : ScriptableObject
{
    public string trapName;
    [Range(0, 5)] public int damage;

    [Range(0, 3f)] public float activeTime = 1f;
    [Range(0, 3f)] public float deactiveTime = 2f;

    public AudioClip activeClip;
}
