using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AgentMovement : MonoBehaviour
{
    private Rigidbody2D _rigid;

    [SerializeField]
    private MovementDataSO _movementSO;

    protected float _currentVelocity = 0;
    protected Vector2 _movementDirection;

    public UnityEvent<float> OnVelocityChange; //플레이어 속도가 바뀔때 실행될 이벤트

    #region 넉백 관련
    [SerializeField]
    protected bool _isKnockBack = false;
    protected Coroutine _knockBackCo = null;
    #endregion

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    public void MoveAgent(Vector2 movementInput)
    {
        if(movementInput.sqrMagnitude > 0)
        {
            if(Vector2.Dot(movementInput, _movementDirection) < 0)
            {
                _currentVelocity = 0;
            }
            _movementDirection = movementInput.normalized;
        }
        _currentVelocity = CalculateSpeed(movementInput);
    }

    private float CalculateSpeed(Vector2 movementInput)
    {
        if(movementInput.sqrMagnitude > 0)
        {
            _currentVelocity += _movementSO.acceleration * Time.deltaTime;
        }else
        {
            _currentVelocity -= _movementSO.deAcceleration * Time.deltaTime;
        }

        return Mathf.Clamp(_currentVelocity, 0, _movementSO.maxSpeed);
    }

    private void FixedUpdate()
    {
        OnVelocityChange?.Invoke(_currentVelocity);
        //넉백이 아닐때만 이동
        if (!_isKnockBack)
            _rigid.velocity = _movementDirection * _currentVelocity;
    }


    //넉백구현할 때 사용할 거다.
    public void StopImmediatelly()
    {
        _currentVelocity = 0;
        _rigid.velocity = Vector2.zero;
    }

    #region 넉백 관련 구현부
    public void KnockBack(Vector2 direction, float power, float duration)
    {
        if (!_isKnockBack)
        {
            _isKnockBack = true;
            _knockBackCo = StartCoroutine(KnockBackCoroutine(direction, power, duration));
        }
    }

    IEnumerator KnockBackCoroutine(Vector2 direction, float power, float duration)
    {
        _rigid.AddForce(direction.normalized * power, ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);
        ResetKnockBackParam();
    }

    public void ResetKnockBackParam()
    {
        _currentVelocity = 0;
        _rigid.velocity = Vector2.zero;
        _isKnockBack = false;
        _rigid.gravityScale = 0;
    }
    #endregion  
}
