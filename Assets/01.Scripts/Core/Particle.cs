using UnityEngine;

public class Particle
{
    private Vector3 _quadPosition;
    private Vector3 _direction;
    private MeshParticleSystem _meshParticleSystem;
    private int _quadIndex;
    private Vector3 _quadSize;
    private float _rotation;
    private int _uvIndex;

    private float _moveSpeed;
    private float _slowDownFactor;

    private bool _isRotate;

    public int QuadIndex { get => _quadIndex; }


    public Particle(Vector3 pos, Vector3 direction, MeshParticleSystem meshParticleSystem, 
                    int uvIndex, float moveSpeed, Vector3 quadSize, float slowDownFactor,
                    bool isRotate = false)
    {
        _quadPosition = pos;
        _direction = direction;
        _meshParticleSystem = meshParticleSystem;
        _isRotate = isRotate;
        _quadSize = quadSize;

        _rotation = Random.Range(0, 360f);

        _uvIndex = uvIndex;

        _moveSpeed = moveSpeed;
        _slowDownFactor = slowDownFactor;

        _quadIndex = _meshParticleSystem.AddQuad(_quadPosition, _rotation, _quadSize, true, _uvIndex);
    }

    public void Update()
    {
        _quadPosition += _direction * _moveSpeed * Time.deltaTime;

        //탄피의 경우 회전도 해야하니
        if (_isRotate)
            _rotation += 360f * (_moveSpeed * 0.1f) * Time.deltaTime;
        _meshParticleSystem.UpdateQuad(_quadIndex, _quadPosition, _rotation, _quadSize, true, _uvIndex);

        //속도 줄여주고
        _moveSpeed -= _moveSpeed * _slowDownFactor * Time.deltaTime;
    }
    public bool IsComplete()
    {
        return _moveSpeed < 0.05f;
    }
}
