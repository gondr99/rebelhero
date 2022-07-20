using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestLineDrawer : MonoBehaviour
{
    public float power = 5f;
    private LineRenderer _lineRenderer;
    private Rigidbody2D _rigid;
    private Vector2 _dragStartPos;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _lineRenderer = GetComponent<LineRenderer>();
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 DragEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 velocity = (_dragStartPos - DragEndPos).normalized * power;

            Vector2[] posList = Plot(_rigid, (Vector2)transform.position, velocity, 800);

            _lineRenderer.positionCount = posList.Length;

            Vector3[] posVector3List =  posList.Select(p => (Vector3)p).ToArray();
            _lineRenderer.SetPositions(posVector3List);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 DragEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 velocity = (_dragStartPos - DragEndPos).normalized * power;

            _rigid.velocity = velocity;
        }
    }

    public Vector2[] Plot(Rigidbody2D rigid, Vector2 pos, Vector2 velocity, int steps)
    {
        Vector2[] results = new Vector2[steps];


        float timeStep = Time.fixedDeltaTime / Physics2D.velocityIterations;
        Vector2 gravityAccel = Physics2D.gravity * rigid.gravityScale * timeStep * timeStep;

        float drag = 1f - timeStep * rigid.drag;

        Vector2 moveStep = velocity * timeStep;

        for(int i = 0; i < steps; i++)
        {
            moveStep += gravityAccel;
            moveStep *= drag;
            pos += moveStep;
            results[i] = pos;
        }
        return results;
    }
}
