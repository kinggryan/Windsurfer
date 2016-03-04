using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereSurfaceSlider))]
public class Storm : MonoBehaviour {

    public float maxTurnRate;
    public float minTurnRate;
    private float turnRate;
    private SphereSurfaceSlider slider;

	// Use this for initialization
	void Start () {
        turnRate = (Random.value > 0.5 ? 1 : -1)*Random.Range(minTurnRate, maxTurnRate);
        slider = GetComponent<SphereSurfaceSlider>();
	}
	
	// Update is called once per frame
	void Update () {
        slider.sphericalVelocity = Quaternion.AngleAxis(turnRate * Time.deltaTime, transform.position)*slider.sphericalVelocity;
	}
}
