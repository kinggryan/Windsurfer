using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour
{
    private float duration = .45f;
    public AnimationCurve shakeCurve;
    private Vector3 shakeDirection;

    private float animationTime = 0f;
    private Vector3 startPosition;
    private float intensity = 0.1f;
    private bool shaking = false;

    private Vector3 originalPosition;

    // Update is called once per frame
    void Update()
    {
        if (shaking)
        {
            animationTime += Time.deltaTime;
            transform.localPosition = originalPosition + shakeCurve.Evaluate(animationTime / duration) * intensity * shakeDirection;
        }
    }

    void StopShaking()
    {
        transform.localPosition = originalPosition;
        shaking = false;
    }

    public void ShakeInDirectionWithIntensity(Vector3 direction, float intensity)
    {
        shaking = true;
        shakeDirection = direction;
        animationTime = 0f;
        this.intensity = intensity;
        Invoke("StopShaking", duration);
        originalPosition = transform.localPosition;
    }
}


