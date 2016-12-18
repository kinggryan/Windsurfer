using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour
{
    public enum ShakeType { Default, Smooth, Rumble };

    private float duration = .45f;
    public AnimationCurve shakeCurve;
    public AnimationCurve smoothShakeCurve;
    public AnimationCurve rumbleShakeCurve;
    private Vector3 shakeDirection;
    private ShakeType shakeType;

    private float animationTime = 0f;
    private Vector3 startPosition;
    private float intensity = 0.1f;
    private bool shaking = false;

    private Vector3 originalPosition;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (shaking)
        {
            animationTime += Time.deltaTime;
            AnimationCurve curveToUse = shakeCurve;
            switch(shakeType)
            {
                case ShakeType.Smooth: curveToUse = smoothShakeCurve; break;
                case ShakeType.Rumble: curveToUse = rumbleShakeCurve; break;
                default: break;
            }
            transform.localPosition = originalPosition + curveToUse.Evaluate(animationTime / duration) * intensity * shakeDirection;
        }
    }

    void StopShaking()
    {
        transform.localPosition = originalPosition;
        shaking = false;
    }

    public void ShakeInDirectionWithIntensity(Vector3 direction, float intensity, ShakeType type = ShakeType.Default)
    {
        if (shaking && type == shakeType && type == ShakeType.Rumble)
            return;

        shaking = true;
        shakeType = type;
        shakeDirection = direction;
        animationTime = 0f;
        this.intensity = intensity;
        Invoke("StopShaking", duration);
        originalPosition = transform.localPosition;
    }

    public void UpdateOriginalRelativeCameraPosition(Vector3 newOriginalPosition)
    {
        originalPosition = newOriginalPosition;
    }
}


