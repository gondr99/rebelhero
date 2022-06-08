using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class EnemyJumpAttack : EnemyAttack
{
    #region 베지어커브관련 코드
    [SerializeField]
    private int _bezeirResolution = 30;
    private Vector3[] _bezierPoints;
    #endregion

    #region 점프 관련 변수들
    [SerializeField]
    private float _jumpSpeed = 0.9f, _jumpDelay = 0.4f, _impactRadius = 2f;
    //점프가 다 완료되는데 걸리는 시간과 점프를 시작하기전까지 걸리는 딜레이
    private float _frameSpeed = 0; //점프 프레임당 걸리는 시간
    
    #endregion

    public UnityEvent PlayJumpAnimation; //점프 시작 애니메이션 재생
    public UnityEvent PlayLandingAnimation; //착지 애니메이션 재생

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
        Vector3 targetPos = GetTarget().position + deltaPos; //점프 지점 구하고
        Vector3 startControl = (targetPos - transform.position) / 4;

        float angle = targetPos.x - transform.position.x < 0 ? -45f : 45f;
        
        Vector3 cp1 = Quaternion.Euler(0, 0, angle) * startControl;  // 1/4지점
        Vector3 cp2 = Quaternion.Euler(0, 0, angle) * (startControl * 3);  // 3/4지점


        _bezierPoints = DOCurve.CubicBezier.GetSegmentPointCloud(transform.position, transform.position + cp1, targetPos, transform.position + cp2, _bezeirResolution);
        _frameSpeed = _jumpSpeed / _bezeirResolution;

        StartCoroutine(JumpCoroutine());

        //디버그용 코드들
        //LineRenderer lr = GetComponent<LineRenderer>();
        //lr.positionCount = bezierPoints.Length;
        //lr.SetPositions(bezierPoints);

    }

    IEnumerator JumpCoroutine()
    {
        AttackFeedback?.Invoke(); //공격사운드 재생후 0.4초후 점프
        yield return new WaitForSeconds(_jumpDelay); 
        PlayJumpAnimation?.Invoke();
        for (int i = 0; i < _bezierPoints.Length; i++)
        {
            yield return new WaitForSeconds(_frameSpeed);
            transform.position = _bezierPoints[i];
            if(i == _bezierPoints.Length - 5)  //종료 5프레임 전이면 랜딩 애니메이션 재생
            {
                EdgeOfEndAnimation();
            }
        }
        JumpEnd();
    }

    //점프가 거의 끝나갈때쯤 재생할 애니메이션
    private void EdgeOfEndAnimation()
    {
        PlayLandingAnimation?.Invoke();
    }

    //점프가 끝나는 시점에서 호출될 코드
    public void JumpEnd()
    {
        ImpactScript impact = PoolManager.Instance.Pop("ImpactShockwave") as ImpactScript;
        Vector3 basePos = _enemyBrain.basePosition.position; // 발바닥을 중심으로 충격파 발생

        float randomRot = Random.Range(0, 360f);
        Quaternion rot = Quaternion.Euler(0, 0, randomRot);

        impact.SetPositionAndRotation(basePos, rot);

        Vector3 dir = GetTarget().position - basePos;
        
        if(dir.sqrMagnitude <= _impactRadius * _impactRadius) //반경내에 들어왔다면
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

        _enemyBrain.SetAttackState(false); //적 두뇌의 공격상태를 해제하고
        
        StartCoroutine(WaitBeforeAttackCoroutine());
    }


    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
