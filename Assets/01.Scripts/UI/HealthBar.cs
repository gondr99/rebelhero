using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private int _healthAmountPerSeparator = 2;
    [SerializeField]
    private float _barSize = 0.94f;
    [SerializeField]
    private Vector3 _sepSize;

    private Transform _barTrm;
    private Transform _separator;
    private Transform _barBackground;

    private int _maxHealth = -1;
    private int _health = -1;

    private MeshFilter _sepMeshFilter;
    private Mesh _sepMesh;
    private MeshRenderer _sepMeshRenderer;

    void Start()
    {
        _barTrm = transform.Find("Bar");
        _barTrm.localScale = new Vector3(0, 1f, 1);

        _separator = transform.Find("SeparatorContainer/Separator");
        _sepMeshFilter = _separator.GetComponent<MeshFilter>();
        _sepMeshRenderer = _separator.GetComponent<MeshRenderer>();

        _sepMeshRenderer.sortingLayerName = "Top";
        _sepMeshRenderer.sortingOrder = 20;
        _barBackground = transform.Find("BarBackground");
        _barSize = _barBackground.localScale.x; //0.94

        gameObject.SetActive(false);
    }

    public void SetHealth(int health)
    {
        if (_maxHealth < 0)
        {
            gameObject.SetActive(true);
            CalculateSeparator(health);
            _maxHealth = health;
        }
        _health = health;

        _barTrm.DOScaleX((float)_health / _maxHealth, 0.7f);
    }

    private void CalculateSeparator(int value)
    {
        _sepMesh = new Mesh();
        SpriteRenderer sr = _barBackground.GetComponent<SpriteRenderer>();
        
        int separatorCnt = Mathf.FloorToInt(value / _healthAmountPerSeparator);

        float boundSize = sr.bounds.size.x;
        float calcSize = (boundSize / separatorCnt) * 0.1f;
        _sepSize.x = Mathf.Min(calcSize, _sepSize.x);

        Vector3[] vertices = new Vector3[(separatorCnt-1) * 4];
        Vector2[] uv = new Vector2[(separatorCnt-1) * 4];
        int[] triangles = new int[(separatorCnt-1) * 6];               

        float barOneGap = _barSize / value;

        for (int i = 0; i < separatorCnt - 1; i++)
        {
            Vector3 pos = new Vector3(barOneGap * (i+1) * _healthAmountPerSeparator, 0, 0);
            int vIndex = i * 4;
            vertices[vIndex + 0] = pos + new Vector3(-_sepSize.x, -_sepSize.y);
            vertices[vIndex + 1] = pos + new Vector3(-_sepSize.x, +_sepSize.y);
            vertices[vIndex + 2] = pos + new Vector3(+_sepSize.x, +_sepSize.y);
            vertices[vIndex + 3] = pos + new Vector3(+_sepSize.x, -_sepSize.y);

            uv[vIndex + 0] = Vector2.zero;
            uv[vIndex + 1] = new Vector2(0, 1);
            uv[vIndex + 2] = Vector2.one;
            uv[vIndex + 3] = new Vector2(1, 0);

            int tIndex = i * 6;
            triangles[tIndex + 0] = vIndex + 0;
            triangles[tIndex + 1] = vIndex + 1;
            triangles[tIndex + 2] = vIndex + 2;
            triangles[tIndex + 3] = vIndex + 0;
            triangles[tIndex + 4] = vIndex + 2;
            triangles[tIndex + 5] = vIndex + 3;
        }
        _sepMesh.vertices = vertices;
        _sepMesh.uv = uv;
        _sepMesh.triangles = triangles;

        _sepMeshFilter.mesh = _sepMesh;
    }
}

