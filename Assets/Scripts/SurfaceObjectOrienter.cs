using UnityEngine;
using System.Collections;

public class SurfaceObjectOrienter : MonoBehaviour {

    public float groundDistance = 75f;

	// Use this for initialization
	void Start () {
        transform.rotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, transform.position.normalized), transform.position.normalized);
        transform.position = groundDistance * transform.position.normalized;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
