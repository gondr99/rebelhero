using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadGagueUI : MonoBehaviour
{
    [SerializeField]
    private Transform reloadBar;
     
    public void ReloadGagueNormal(float value)
    {
        reloadBar.transform.localScale = new Vector3(value, 1, 1);
    }
}
