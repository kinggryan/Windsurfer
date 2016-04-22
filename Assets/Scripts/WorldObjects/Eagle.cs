using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereSurfaceSlider))]
public class Eagle : MonoBehaviour {

    public float maxTurnSpeed;
    public Transform target;

    private SphereSurfaceSlider slider;

	// Use this for initialization
	void Start () {
        if(target == null)
            target = Object.FindObjectOfType<PlayerMouseController>().transform;

        slider = GetComponent<SphereSurfaceSlider>();
	}
	
	// Update is called once per frame
	void Update () {
        slider.sphericalVelocity = Vector3.RotateTowards(slider.sphericalVelocity, slider.sphericalVelocity.magnitude*Vector3.Cross(transform.position.normalized, target.position.normalized).normalized, maxTurnSpeed*Mathf.Deg2Rad*Time.deltaTime, Mathf.Infinity);
	}
}
