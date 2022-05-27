using UnityEngine;

public class TimeFreezeFeedback : Feedback
{
    [SerializeField]
    private float _freezeTimeDelay = 0.05f, _unFreezeTimeDelay = 0.02f;

    [SerializeField]
    [Range(0, 1f)]
    private float _timeFreezeValue = 0.2f;


    public override void CompletePrevFeedback()
    {
        if (TimeController.Instance != null)
            TimeController.Instance.ResetTimeScale();
    }

    public override void CreateFeedback()
    {
        // 0.05���Ŀ� 0.2�� Ÿ���� �����ߴٰ� �ٽ� 0.02�� �Ŀ� 1�� �����Ѵ�.
        TimeController.Instance.ModifyTimeScale(_timeFreezeValue, _freezeTimeDelay, () =>
        {
            TimeController.Instance.ModifyTimeScale(1, _unFreezeTimeDelay);
        });
    }
}
