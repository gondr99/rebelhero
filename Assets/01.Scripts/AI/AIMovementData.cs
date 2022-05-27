using UnityEngine;

public class AIMovementData : MonoBehaviour
{
    [field: SerializeField]
    public Vector2 direction { get; set; }

    [field: SerializeField]
    public Vector2 pointOfInterest { get; set; }
    //관심이 있는 지점. 도망갈건지 먹을건지 등등을 결정하게 만드는 요소

}