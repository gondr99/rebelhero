using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[System.Serializable]
[CreateAssetMenu(menuName = "SO/Items/ResourceData")]
public class ResourceDataSO : ScriptableObject
{
    public float rate;
    public GameObject itemPrefab;

    [field: SerializeField]
    public ResourceTypeEnum ResourceType { get; set; }
    [SerializeField]
    private int minAmount = 1, maxAmount = 5;

    public AudioClip useSound;

    public Color popupTextColor;

    public int GetAmount()
    {
        return Random.Range(minAmount, maxAmount + 1);
    }
}