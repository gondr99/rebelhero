using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class FlashLightFeedback : Feedback
{
    [SerializeField]
    private Light2D _lightTarget = null;
    [SerializeField]
    private float _lightOnDelay = 0.01f, _lightOffDelay = 0.01f;
    [SerializeField]
    private bool _defaultState = false;

    public override void CompletePrevFeedback()
    {
        StopAllCoroutines();
        _lightTarget.enabled = _defaultState;
    }

    IEnumerator ToggleLightCoroutine(float time, bool result, Action FinishCallback = null)
    {
        yield return new WaitForSeconds(time);
        _lightTarget.enabled = result;
        FinishCallback?.Invoke();
    }

    public override void CreateFeedback()
    {
        StartCoroutine(ToggleLightCoroutine(_lightOnDelay, true, () =>
        {
            StartCoroutine(ToggleLightCoroutine(_lightOffDelay, false));
        }));
    }
}
