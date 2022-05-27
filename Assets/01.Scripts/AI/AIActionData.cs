using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIActionData : MonoBehaviour
{
    [field: SerializeField]
    public bool attack { get; set; }

    [field: SerializeField]
    public bool targetSpotted { get; set; }

    [field: SerializeField]
    public bool arrived { get; set; }
}

