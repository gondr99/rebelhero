using UnityEngine;

public class AgentSound : AudioPlayer
{
    [SerializeField]
    private AudioClip _hitClip = null, _deathClip = null, _voiceLineclip = null, _attackSound = null;
    // 피격, 죽음, 발견시의 사운드
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
