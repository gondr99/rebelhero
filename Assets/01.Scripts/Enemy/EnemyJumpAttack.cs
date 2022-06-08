using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class EnemyJumpAttack : EnemyAttack
{
    #region ������Ŀ����� �ڵ�
    [SerializeField]
    private int _bezeirResolution = 30;
    private Vector3[] _bezierPoints;
    #endregion

    #region ���� ���� ������
    [SerializeField]
    private float _jumpSpeed = 0.9f, _jumpDelay = 0.4f, _impactRadius = 2f;
    //������ �� �Ϸ�Ǵµ� �ɸ��� �ð��� ������ �����ϱ������� �ɸ��� ������
    private float _frameSpeed = 0; //���� �����Ӵ� �ɸ��� �ð�
    
    #endregion

    public UnityEvent PlayJumpAnimation; //���� ���� �ִϸ��̼� ���
    public UnityEvent PlayLandingAnimation; //���� �ִϸ��̼� ���

    public override void Attack(int damage)
    {
        if (_waitBeforeNextAttack == false)
        {
            _enemyBrain.SetAttackState(true);

            Jump();
        }
    }

    private void Jump()
    {
     
        _waitBeforeNextAttack = true;
        Vector3 deltaPos = transform.position - _enemyBrain.basePosition.position;
        Vector3 targetPos = GetTarget().position + deltaPos; //���� ���� ���ϰ�
        Vector3 startControl = (targetPos - transform.position) / 4;

        float angle = targetPos.x - transform.position.x < 0 ? -45f : 45f;
        
        Vector3 cp1 = Quaternion.Euler(0, 0, angle) * startControl;  // 1/4����
        Vector3 cp2 = Quaternion.Euler(0, 0, angle) * (startControl * 3);  // 3/4����


        _bezierPoints = DOCurve.CubicBezier.GetSegmentPointCloud(transform.position, transform.position + cp1, targetPos, transform.position + cp2, _bezeirResolution);
        _frameSpeed = _jumpSpeed / _bezeirResolution;

        StartCoroutine(JumpCoroutine());

        //����׿� �ڵ��
        //LineRenderer lr = GetComponent<LineRenderer>();
        //lr.positionCount = bezierPoints.Length;
        //lr.SetPositions(bezierPoints);

    }

    IEnumerator JumpCoroutine()
    {
        AttackFeedback?.Invoke(); //���ݻ��� ����� 0.4���� ����
        yield return new WaitForSeconds(_jumpDelay); 
        PlayJumpAnimation?.Invoke();
        for (int i = 0; i < _bezierPoints.Length; i++)
        {
            yield return new WaitForSeconds(_frameSpeed);
            transform.position = _bezierPoints[i];
            if(i == _bezierPoints.Length - 5)  //���� 5������ ���̸� ���� �ִϸ��̼� ���
            {
                EdgeOfEndAnimation();
            }
        }
        JumpEnd();
    }

    //������ ���� ���������� ����� �ִϸ��̼�
    private void EdgeOfEndAnimation()
    {
        PlayLandingAnimation?.Invoke();
    }

    //������ ������ �������� ȣ��� �ڵ�
    public void JumpEnd()
    {
        ImpactScript impact = PoolManager.Instance.Pop("ImpactShockwave") as ImpactScript;
        Vector3 basePos = _enemyBrain.basePosition.position; // �߹ٴ��� �߽����� ����� �߻�

        float randomRot = Random.Range(0, 360f);
        Quaternion rot = Quaternion.Euler(0, 0, randomRot);

        impact.SetPositionAndRotation(basePos, rot);

        Vector3 dir = GetTarget().position - basePos;
        
        if(dir.sqrMagnitude <= _impactRadius * _impactRadius) //�ݰ泻�� ���Դٸ�
        {
            IHittable targetHit = GetTarget().GetComponent<IHittable>();
            targetHit?.GetHit(_enemy.EnemyData.damage, gameObject);

            if(dir.sqrMagnitude == 0)
            {
                dir = Random.insideUnitCircle;
            }
            IKnockBack targetKnockback = GetTarget().GetComponent<IKnockBack>();
            targetKnockback?.KnockBack(dir.normalized, 5f, 1f);
        }

        _enemyBrain.SetAttackState(false); //�� �γ��� ���ݻ��¸� �����ϰ�
        
        StartCoroutine(WaitBeforeAttackCoroutine());
    }


    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
