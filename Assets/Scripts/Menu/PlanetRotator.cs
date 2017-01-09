using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotator : MonoBehaviour {

	public Transform objToRotateAround;
	public float rotationSpeed;

	// Use this for initialization
	void Start () {
		if (objToRotateAround == null)
			objToRotateAround = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround (objToRotateAround.position, objToRotateAround.up, rotationSpeed * Time.deltaTime);
	}
}
