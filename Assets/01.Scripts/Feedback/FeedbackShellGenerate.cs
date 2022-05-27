using UnityEngine;
using UnityEngine.Events;

public class FeedbackShellGenerate : Feedback
{
    [field: SerializeField]
    private UnityEvent ShellGenerated = null;

    private IRangeWeapon _weapon;

    private void Awake()
    {
        _weapon = transform.parent.GetComponent<IRangeWeapon>();
    }

    public override void CompletePrevFeedback()
    {
        if (_weapon == null) return;
        //do nothing here
    }

    public override void CreateFeedback()
    {
        if (_weapon == null) return;
        Vector3 shellPos = _weapon.GetShellEjectPostion();
        Vector3 ejectDir = _weapon.GetEjectDirection();

        TextureParticleManager.Instance.SpawnShell(shellPos, ejectDir);
        ShellGenerated?.Invoke();
    }
}
