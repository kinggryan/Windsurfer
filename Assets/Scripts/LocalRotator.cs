using UnityEngine;
using System.Collections;

public class LocalRotator : MonoBehaviour {

    public float rotationRate = 25f;

	// Update is called once per frame
	void Update () {
        transform.RotateAround(Vector3.zero, transform.up, Time.deltaTime * rotationRate);
    }
}
