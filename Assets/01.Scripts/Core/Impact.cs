using UnityEngine;

public class Impact
{
    private ImpactMeshSystem _meshSystem;
    private int _quadIndex;
    private int _totalIdx;
    private int _currentIdx;
    private float _timeToPlay; //애니메이션이 한사이클 도는데 걸리는 시간
    private float _frameSec;
    private float _currentTime;
    private Vector3 _position;
    private float _rotation;
    private Vector3 _quadSize;
    private bool _skew;
    private int _impactIdx;

    private bool _isComplete;
    public bool IsComplete
    {
        get => _isComplete;
    }


    public Impact(ImpactMeshSystem meshSystem, Vector3 position, float rotation, Vector3 quadSize, bool skew, int impactIdx, int total, float time)
    {
        
        _meshSystem = meshSystem;
        _position = position;
        _rotation = rotation;
        _quadSize = quadSize;
        _skew = skew;
        _impactIdx = impactIdx;

        _totalIdx = total;
        _timeToPlay = time;
        _frameSec = time / _totalIdx;
        _currentTime = 0;
        _currentIdx = 0;
        _isComplete = false;

        _quadIndex = meshSystem.AddQuad(_position, _rotation, _quadSize, true, impactIdx, 0);
    }

    public void Update()
    {
        _currentTime += Time.deltaTime;
        if (_currentTime >= _frameSec)
        {
       
            _currentTime = 0;
            _currentIdx++;
            if (_currentIdx >= _totalIdx)
            {
                _isComplete = true;
                _meshSystem.DestroyQuad(_quadIndex);
                return;
            }

            _meshSystem.UpdateQuad(_quadIndex, _position, _rotation, _quadSize, _skew, _impactIdx, _currentIdx);
        }
    }

}
