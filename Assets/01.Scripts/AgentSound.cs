using UnityEngine;

public class AgentSound : AudioPlayer
{
    [SerializeField]
    private AudioClip _hitClip = null, _deathClip = null, _voiceLineclip = null, _attackSound = null;
    // �ǰ�, ����, �߽߰��� ����
    public void PlayHitSound()
    {
        PlayClipWithVariablePitch(_hitClip);
    }

    public void PlayDeathSound()
    {
        PlayClip(_deathClip);
    }

    public void PlayVoiceSound()
    {
        PlayClipWithVariablePitch(_voiceLineclip);
    }

    public void PlayAttackSound()
    {
        PlayClip(_attackSound);
    }

}
