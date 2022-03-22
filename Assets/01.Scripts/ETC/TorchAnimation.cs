using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Experimental.Rendering.Universal;

public class TorchAnimation : MonoBehaviour
{
    [SerializeField]
    private bool _changeRadius; //흔들릴때 반경까지 흔들릴꺼냐?

    [SerializeField] private float _intensityRandomness;
    [SerializeField] private float _radiusRandomness;
    [SerializeField] private float _timeRandomness;

    private float _baseIntensity;
    private float _baseTime = 0.5f;
    private float _baseRadius;

    private Light2D _light;

    private void Awake()
    {
        _light = GetComponentInChildren<Light2D>();
        _baseIntensity = _light.intensity;
        _baseRadius = _light.pointLightOuterRadius;
    }

    private void Start()
    {
        ShakeLight();
    }
    //방과후를 
    private void ShakeLight()
    {
        //닷트윈 시퀀스를 이용해서 여기서 한번 횟불을 흔들꺼야
        float targetIntensity = _baseIntensity + Random.Range(-_intensityRandomness, _intensityRandomness);
        float targetTime = _baseTime + Random.Range(-_timeRandomness, _timeRandomness);


        Sequence seq = DOTween.Sequence();
        seq.Append(DOTween.To(
            () => _light.intensity,
            value => _light.intensity = value,
            targetIntensity,
            targetTime
        ));

        //반지름도 흔들고 싶다면 여기도 작성해보자.
        if (_changeRadius)
        {
            //seq.Append()
        }

        seq.AppendCallback(() => ShakeLight());

        //시퀀스의 마지막에는 (한번 흔들린 다음에는) 다시한번 ShakeLight

    }
}
