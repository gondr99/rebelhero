using UnityEngine;
using DG.Tweening;

public class ShakeFeedback : Feedback
{
    [SerializeField]
    private GameObject _objectToShake;
    [SerializeField]
    private float _duration = 0.2f, _strength = 1f, _randomness = 90;
    [SerializeField]
    private int _vibrato = 10;
    [SerializeField]
    private bool _snapping = false, _fadeOut = true; 
    //fade아웃은 쉐이킹 후 원래대로 돌아가는 옵션
    public override void CompletePrevFeedback()
    {
        _objectToShake.transform.DOComplete();
        //모든 트윈을 즉시 완료시키고 완료된 트윈의 수를 반납한다.
    }

    public override void CreateFeedback()
    {
        CompletePrevFeedback();
        _objectToShake.transform.DOShakePosition(_duration, _strength, _vibrato, _randomness, _snapping, _fadeOut);
    }
}
