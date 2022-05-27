using UnityEngine;

public class EnemyAnimation : AgentAnimation
{
    protected EnemyAIBrain _enemyAIBrain;
    protected readonly int _attackHashStr = Animator.StringToHash("Attack");

    protected override void ChildAwake()
    {
        _enemyAIBrain = transform.parent.GetComponent<EnemyAIBrain>();
    }

    public void SetEndOfAttackAnimation()
    {
        _enemyAIBrain.SetAttackState(false);
    }

    public void PlayAttackAnimation()
    {
        _agentAnimator.SetTrigger(_attackHashStr);
    }

}
