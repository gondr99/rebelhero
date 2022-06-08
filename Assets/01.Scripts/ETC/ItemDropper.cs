using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemDropper : MonoBehaviour
{
    [SerializeField] 
    private ItemDropTableSO _dropTable;
    private float[] _itemWeights;

    [SerializeField]
    private bool _dropEffect = false; //���� ����Ʈ 
    [SerializeField]
    private float _dropPower = 2f;

    [SerializeField]
    [Range(0, 1)]
    private float _dropChance;

    private void Start()
    {
        //����Ʈ�� �ִ� items���� rate�� ���� �̾Ƽ� �迭�� ������ش�.
        _itemWeights = _dropTable.dropList.Select(item => item.rate).ToArray();

    }

    public void DropItem()
    {
        float dropVariable = Random.value;
        if (dropVariable < _dropChance) //��������� �ɷȴٸ�
        {
            int index = GetRandomWeightedIndex();
            PoolableMono resource = PoolManager.Instance.Pop(_dropTable.dropList[index].itemPrefab.name);
            resource.transform.position = transform.position;

            Vector3 offset = Random.insideUnitCircle;

            if (_dropEffect)
            {
                resource.transform.DOJump(transform.position + offset, _dropPower, 1, 0.3f);
            }
        }
    }

    private int GetRandomWeightedIndex()
    {
        float sum = 0f;
        for (int i = 0; i < _itemWeights.Length; i++)
        {
            sum += _itemWeights[i];
        }

        float randomValue = Random.Range(0f, sum);
        float tempSum = 0f;

        for (int i = 0; i < _itemWeights.Length; i++)
        {
            if (randomValue >= tempSum && randomValue < tempSum + _itemWeights[i])
            {
                return i;
            }
            else
            {
                tempSum += _itemWeights[i];
            }
        }

        return 0;
    }
}