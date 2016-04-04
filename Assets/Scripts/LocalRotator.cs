using UnityEngine;
using System.Collections;

public class LocalRotator : MonoBehaviour {

    public float rotationRate = 25f;
    public bool rotateAroundSelf = false;
    public Vector3 relativeVector = Vector3.up;

	// Update is called once per frame
	void Update () {
        transform.RotateAround(rotateAroundSelf ? transform.position : Vector3.zero, transform.TransformDirection(relativeVector), Time.deltaTime * rotationRate);
    }
}
