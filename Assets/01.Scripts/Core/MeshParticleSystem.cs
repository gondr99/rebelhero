using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeshParticleSystem : MonoBehaviour
{
    private const int MAX_QUAD_AMOUNT = 15000; //최대 15000개의 영구 파티클을 가지고 있을 수 있다.
    
    [Serializable]
    public struct ParticleUVPixel
    {
        public Vector2Int uv00Pixel;  //좌측하단
        public Vector2Int uv11Pixel; //우측상단
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

    #region 쿼드 생성 및 업데이트 관련 변수
    private int _quadIndex = 0;
    private bool _updateVertices;
    private bool _updateUV;
    private bool _updateTriangles;
    #endregion

    private void Awake()
    {
        _mesh = new Mesh();

        //최대 쿼드의 갯수에 알맞게 정점과 삼각형의 갯수를 셋팅
        _vertices = new Vector3[4 * MAX_QUAD_AMOUNT];
        _uv = new Vector2[4 * MAX_QUAD_AMOUNT];
        _triangles = new int[6 * MAX_QUAD_AMOUNT];

        _mesh.vertices = _vertices;
        _mesh.uv = _uv;
        _mesh.triangles = _triangles;

        //메시의 경계 바운드가 작으면 특정 좌표이상 화면 밖으로 나가면 메시 전체가 안그려지는 문제가 생김.
        _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10000f);

        //필요한 컴포넌트들을 가져와서 셋팅
        _meshFilter = GetComponent<MeshFilter>();
        _meshFilter.mesh = _mesh;
        _meshRenderer = GetComponent<MeshRenderer>();

        //이 값은 코드로만접근 가능
        _meshRenderer.sortingLayerName = "Agent";
        _meshRenderer.sortingOrder = 0;


        //메인텍스쳐의 최대 width와 height를 알아내야하기 때문에. 
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

        _uvCoordArr = uvCoordList.ToArray(); //배열로 변환해서 참조할때 속도 조금 더 빠르게
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
        //실질적인 쿼드 스폰
        UpdateQuad(_quadIndex, position, rotation, quadSize, skewed, uvIndex);
        int spawnedQuadIndex = _quadIndex;
        _quadIndex = (_quadIndex + 1) % MAX_QUAD_AMOUNT; //최대치 초과하면 이전께 지워지도록 처리

        return spawnedQuadIndex;
    }

    public void UpdateQuad(int quadIndex, Vector3 position, float rotation, Vector3 quadSize, bool skewed, int uvIndex)
    {
        // 4배수로 올라가니까
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
         *   순서로 사각형을 맵핑시킨다
         */
        //uV 맵핑 - 노말라이즈된 값을 맵핑하면 됨.
        UVCoords uv = _uvCoordArr[uvIndex];
        _uv[vIndex0] = uv.uv00;
        _uv[vIndex1] = new Vector2(uv.uv00.x, uv.uv11.y);
        _uv[vIndex2] = uv.uv11;
        _uv[vIndex3] = new Vector2(uv.uv11.x, uv.uv00.y);

        //Create Triangle
        int tIndex = quadIndex * 6; //사각형 하나당 2개의 삼각형이고 삼각형은 3개의 꼭지점이라
        //트라이앵글 정점매칭은 시계방향으로 해야 정방
        _triangles[tIndex + 0] = vIndex0; // -1, -1
        _triangles[tIndex + 1] = vIndex1; // -1, 1
        _triangles[tIndex + 2] = vIndex2; // 1, 1

        _triangles[tIndex + 3] = vIndex0;  // -1, -1
        _triangles[tIndex + 4] = vIndex2;  // 1, 1
        _triangles[tIndex + 5] = vIndex3;  // 1, -1

        //변경이 생겼을 때 재반영해주지 않으면 변경없음으로 이해하고 화면에 반영하지 않는다.
        //하지만 이 작업을 매 프레임에 여러개의 쿼드에 다 해주니까 동일한 작업이 프레임에 여러번 이뤄진다. 그래서 boolean 변수를 통해 한번만
        // TextMeshPro의 ForceMeshUpdate가 이 역할이다.
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
        //만약 최대치를 돌아서 갔다면 이거 작동안한다. 
        // 다만 모든경우마다 모든것을 지우기에는 너무 오버헤드가 심하니 
        // 어지간하면 15000개 안넘어갈거라 가정하고 간다..-_-;
        Array.Clear(_vertices, 0, _vertices.Length);
        _quadIndex = 0;
        _updateVertices = true;
    }
}
