using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMove : MonoBehaviour
{
    private Transform _centerTrm;
    private Transform _outerTrm;

    private float _angle;
    [SerializeField] private float _speed = 180f;

    private void Awake()
    {
        _centerTrm = transform.Find("Blue");
        _outerTrm = transform.Find("Red");
        _angle = 180f;
    }

    private void Update()
    {
        Vector3 dir = _outerTrm.position - _centerTrm.position;

        dir = Quaternion.Euler(0, 0, Time.deltaTime * _speed) * dir;
        _outerTrm.position = _centerTrm.position + dir;


        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeCircle();
        }
    }

    private void ChangeCircle()
    {
        Transform temp = _outerTrm;
        _outerTrm = _centerTrm;
        _centerTrm = temp;
    }
}
