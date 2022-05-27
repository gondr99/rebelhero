using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TestMove : MonoBehaviour
{
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            worldPos.z = 0;

            Vector3 one = Vector3.Lerp(transform.position, worldPos, 0.25f);
            Vector3 two = Vector3.Lerp(transform.position, worldPos, 0.75f);
            one.y += 2;

            two.y -= 2;

            Vector3[] arr =  DOCurve.CubicBezier.GetSegmentPointCloud(
                transform.position, one, 
                worldPos,two , 100);

            StartCoroutine(StartMove(arr));
        }
    }

    IEnumerator StartMove(Vector3[] arr)
    {
        for(int i = 0; i < arr.Length; i++)
        {
            yield return null;
            transform.position = arr[i];
        }
    }
}
