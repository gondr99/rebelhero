using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeshParticleSystem : MonoBehaviour
{
    private const int MAX_QUAD_AMOUNT = 15000; //�ִ� 15000���� ���� ��ƼŬ�� ������ ���� �� �ִ�.
    
    [Serializable]
    public struct ParticleUVPixel
    {
        public Vector2Int uv00Pixel;  //�����ϴ�
        public Vector2Int uv11Pixel; //�������
    }

    public struct UVCoords
    {
        public Vector2 uv00;
        public Vector2 uv11;
    }
    [SerializeField]
    private ParticleUVPixel[] _uvPixelArr;
    private UVCoords[] _uvCoordArr;

    private Mesh _mesh;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    private Vector3[] _vertices;
    private Vector2[] _uv;
    private int[] _triangles;

    #region ���� ���� �� ������Ʈ ���� ����
    private int _quadIndex = 0;
    private bool _updateVertices;
    private bool _updateUV;
    private bool _updateTriangles;
    #endregion

    private void Awake()
    {
        _mesh = new Mesh();

        //�ִ� ������ ������ �˸°� ������ �ﰢ���� ������ ����
        _vertices = new Vector3[4 * MAX_QUAD_AMOUNT];
        _uv = new Vector2[4 * MAX_QUAD_AMOUNT];
        _triangles = new int[6 * MAX_QUAD_AMOUNT];

        _mesh.vertices = _vertices;
        _mesh.uv = _uv;
        _mesh.triangles = _triangles;

        //�޽��� ��� �ٿ�尡 ������ Ư�� ��ǥ�̻� ȭ�� ������ ������ �޽� ��ü�� �ȱ׷����� ������ ����.
        _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10000f);

        //�ʿ��� ������Ʈ���� �����ͼ� ����
        _meshFilter = GetComponent<MeshFilter>();
        _meshFilter.mesh = _mesh;
        _meshRenderer = GetComponent<MeshRenderer>();

        //�� ���� �ڵ�θ����� ����
        _meshRenderer.sortingLayerName = "Agent";
        _meshRenderer.sortingOrder = 0;


        //�����ؽ����� �ִ� width�� height�� �˾Ƴ����ϱ� ������. 
        Texture mainTex = _meshRenderer.material.mainTexture;

        int tWidth = mainTex.width;
        int tHeight = mainTex.height;

        List<UVCoords> uvCoordList = new List<UVCoords>();

        foreach(ParticleUVPixel pixelUV in _uvPixelArr)
        {
            UVCoords temp = new UVCoords
            {
                uv00 = new Vector2((float)pixelUV.uv00Pixel.x / tWidth, (float)pixelUV.uv00Pixel.y / tHeight),
                uv11 = new Vector2((float)pixelUV.uv11Pixel.x / tWidth, (float)pixelUV.uv11Pixel.y / tHeight)
            };

            uvCoordList.Add(temp);
        }

        _uvCoordArr = uvCoordList.ToArray(); //�迭�� ��ȯ�ؼ� �����Ҷ� �ӵ� ���� �� ������
    }

    public int GetRandomBloodIndex()
    {
        return Random.Range(0, 8);
    }

    public int GetRandomShellIndex()
    {
        return Random.Range(8, 9);
    }

    private void LateUpdate()
    {
        if (_updateVertices)
        {
            _mesh.vertices = _vertices;
            _updateVertices = false;
        }

        if (_updateUV)
        {
            _mesh.uv = _uv;
            _updateUV = false;
        }

        if (_updateTriangles)
        {
            _mesh.triangles = _triangles;
            _updateTriangles = false;
        }
    }


    public int AddQuad(Vector3 position, float rotation, Vector3 quadSize, bool skewed, int uvIndex)
    {
        //�������� ���� ����
        UpdateQuad(_quadIndex, position, rotation, quadSize, skewed, uvIndex);
        int spawnedQuadIndex = _quadIndex;
        _quadIndex = (_quadIndex + 1) % MAX_QUAD_AMOUNT; //�ִ�ġ �ʰ��ϸ� ������ ���������� ó��

        return spawnedQuadIndex;
    }

    public void UpdateQuad(int quadIndex, Vector3 position, float rotation, Vector3 quadSize, bool skewed, int uvIndex)
    {
        // 4����� �ö󰡴ϱ�
        int vIndex0 = quadIndex * 4;
        int vIndex1 = vIndex0 + 1;
        int vIndex2 = vIndex0 + 2;
        int vIndex3 = vIndex0 + 3;

        if(skewed)
        {
            _vertices[vIndex0] = position + Quaternion.Euler(0, 0, rotation) * new Vector3(-quadSize.x, -quadSize.y);
            _vertices[vIndex1] = position + Quaternion.Euler(0, 0, rotation) * new Vector3(-quadSize.x, +quadSize.y);
            _vertices[vIndex2] = position + Quaternion.Euler(0, 0, rotation) * new Vector3(+quadSize.x, +quadSize.y);
            _vertices[vIndex3] = position + Quaternion.Euler(0, 0, rotation) * new Vector3(+quadSize.x, -quadSize.y);
        }
        else
        {
            _vertices[vIndex0] = position + Quaternion.Euler(0, 0, rotation - 180) * quadSize; // -1, -1
            _vertices[vIndex1] = position + Quaternion.Euler(0, 0, rotation - 270) * quadSize; // -1, 1
            _vertices[vIndex2] = position + Quaternion.Euler(0, 0, rotation - 0) * quadSize; // -1, 1
            _vertices[vIndex3] = position + Quaternion.Euler(0, 0, rotation - 90) * quadSize; // -1, 1
        }

        /*
         *   1 - 2
         *   |   |
         *   0 - 3
         *   ������ �簢���� ���ν�Ų��
         */
        //uV ���� - �븻������� ���� �����ϸ� ��.
        UVCoords uv = _uvCoordArr[uvIndex];
        _uv[vIndex0] = uv.uv00;
        _uv[vIndex1] = new Vector2(uv.uv00.x, uv.uv11.y);
        _uv[vIndex2] = uv.uv11;
        _uv[vIndex3] = new Vector2(uv.uv11.x, uv.uv00.y);

        //Create Triangle
        int tIndex = quadIndex * 6; //�簢�� �ϳ��� 2���� �ﰢ���̰� �ﰢ���� 3���� �������̶�
        //Ʈ���̾ޱ� ������Ī�� �ð�������� �ؾ� ����
        _triangles[tIndex + 0] = vIndex0; // -1, -1
        _triangles[tIndex + 1] = vIndex1; // -1, 1
        _triangles[tIndex + 2] = vIndex2; // 1, 1

        _triangles[tIndex + 3] = vIndex0;  // -1, -1
        _triangles[tIndex + 4] = vIndex2;  // 1, 1
        _triangles[tIndex + 5] = vIndex3;  // 1, -1

        //������ ������ �� ��ݿ������� ������ ����������� �����ϰ� ȭ�鿡 �ݿ����� �ʴ´�.
        //������ �� �۾��� �� �����ӿ� �������� ���忡 �� ���ִϱ� ������ �۾��� �����ӿ� ������ �̷�����. �׷��� boolean ������ ���� �ѹ���
        // TextMeshPro�� ForceMeshUpdate�� �� �����̴�.
        _updateVertices = true;
        _updateUV = true;
        _updateTriangles = true;
    }

    public void DestroyQuad(int quadIndex)
    {
        int vIndex = quadIndex * 4;
        int vIndex0 = vIndex;
        int vIndex1 = vIndex + 1;
        int vIndex2 = vIndex + 2;
        int vIndex3 = vIndex + 3;

        _vertices[vIndex0] = Vector3.zero;
        _vertices[vIndex1] = Vector3.zero;
        _vertices[vIndex2] = Vector3.zero;
        _vertices[vIndex3] = Vector3.zero;

        _updateVertices = true;
    }

    public void DestroyAllQuad()
    {
        //���� �ִ�ġ�� ���Ƽ� ���ٸ� �̰� �۵����Ѵ�. 
        // �ٸ� ����츶�� ������ ����⿡�� �ʹ� ������尡 ���ϴ� 
        // �������ϸ� 15000�� �ȳѾ�Ŷ� �����ϰ� ����..-_-;
        Array.Clear(_vertices, 0, _vertices.Length);
        _quadIndex = 0;
        _updateVertices = true;
    }
}
