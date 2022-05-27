using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTest : MonoBehaviour
{
    private float currentAngle = 0;
    private float targetAngle;
    [SerializeField]
    private float rotateSpeed = Mathf.Deg2Rad * 60f;

    private Vector2 lastDir;
    private float _currentSpeed;
    private Vector2 _beforeDir;

    private Rigidbody2D _rigid;
    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        NewMovement();
        //OldMovement();

    }

    private void NewMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y).normalized;

        _beforeDir = Vector2.Lerp(_beforeDir, Vector2.zero, Time.deltaTime * 3);

        Vector3 nextDir = _beforeDir + dir * 1;

        _rigid.velocity = nextDir;
        _beforeDir = nextDir;
    }

    private void OldMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y).normalized;

        if (dir.magnitude <= 0)
        {
            if (_currentSpeed > 0)
                _currentSpeed -= Time.deltaTime * 2f;
            else
                _currentSpeed = 0;
            _rigid.velocity = _beforeDir * _currentSpeed;
            return;
        }

        targetAngle = Mathf.Atan2(dir.y, dir.x);

        if (currentAngle < targetAngle)
        {
            currentAngle += rotateSpeed * Time.deltaTime;
        }
        else if (currentAngle > targetAngle)
        {
            currentAngle -= rotateSpeed * Time.deltaTime;
        }

        _beforeDir = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle)).normalized;

        _currentSpeed = 2f;

        _rigid.velocity = _beforeDir * _currentSpeed;

        Debug.Log(dir);

        lastDir = dir;
    }
}
