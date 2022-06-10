using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextMove : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textPro;

    private void Awake()
    {
        _textPro = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {

        _textPro.ForceMeshUpdate();
        TMP_TextInfo textInfo = _textPro.textInfo;

        TMP_CharacterInfo[] tmpCharInfo = textInfo.characterInfo;

        for (int i = 0; i < 3; i++)
        {
            TMP_CharacterInfo charInfo = tmpCharInfo[i];
            int matIdx = charInfo.materialReferenceIndex;
            Vector3[] destinationVertices = textInfo.meshInfo[matIdx].vertices;
            int vertexIndex = charInfo.vertexIndex;
            destinationVertices[vertexIndex + 0] += new Vector3(0, 100f, 0);
            destinationVertices[vertexIndex + 1] += new Vector3(0, 100f, 0);
            destinationVertices[vertexIndex + 2] += new Vector3(0, 100f, 0);
            destinationVertices[vertexIndex + 3] += new Vector3(0, 100f, 0);


        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            _textPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }

    }
}
