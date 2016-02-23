using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

    public float rotationSpeed = 360f;
    public float maximumRotationAcceleration = 360f;

    private Vector2 lastMovementSpeed = Vector2.zero;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 movementNorm = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        movementNorm.Normalize();
        
        Vector2 movementActual = Vector2.MoveTowards(lastMovementSpeed, movementNorm * rotationSpeed,maximumRotationAcceleration*Time.deltaTime);
        lastMovementSpeed = movementActual;

        transform.RotateAround(Vector3.zero, transform.up, -1*movementActual.x*Time.deltaTime);
        transform.RotateAround(Vector3.zero, transform.right, movementActual.y * Time.deltaTime);
	}

}
