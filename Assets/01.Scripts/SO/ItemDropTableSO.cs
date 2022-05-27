using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Items/DropTable")]
public class ItemDropTableSO : ScriptableObject
{
    public List<ResourceDataSO> dropList;
}