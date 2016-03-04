using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereSurfaceSlider))]
public class SphericalDirectionTurner : MonoBehaviour {

    public float maxTurnRate;
    public AnimationCurve turnRateCurve;
    public float turnRateChangeCycleDuration;

    private float turnRate;
    private SphereSurfaceSlider slider;
    private float turnRateChangeCyclePosition;

	// Use this for initialization
	void Start () {
        slider = GetComponent<SphereSurfaceSlider>();
        turnRateChangeCyclePosition = Random.value;
	}
	
	// Update is called once per frame
	void Update () {
        slider.sphericalVelocity = Quaternion.AngleAxis(turnRate * Time.deltaTime, transform.position)*slider.sphericalVelocity;

        turnRateChangeCyclePosition += Time.deltaTime/turnRateChangeCycleDuration;
        if (turnRateChangeCyclePosition >= 1)
            turnRateChangeCyclePosition -= 1;
        turnRate = (maxTurnRate)*(2*turnRateCurve.Evaluate(turnRateChangeCyclePosition) - 1);
	}
}
