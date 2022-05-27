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
    //fade�ƿ��� ����ŷ �� ������� ���ư��� �ɼ�
    public override void CompletePrevFeedback()
    {
        _objectToShake.transform.DOComplete();
        //��� Ʈ���� ��� �Ϸ��Ű�� �Ϸ�� Ʈ���� ���� �ݳ��Ѵ�.
    }

    public override void CreateFeedback()
    {
        CompletePrevFeedback();
        _objectToShake.transform.DOShakePosition(_duration, _strength, _vibrato, _randomness, _snapping, _fadeOut);
    }
}
