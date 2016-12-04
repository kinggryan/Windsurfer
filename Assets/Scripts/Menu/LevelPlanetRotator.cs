using UnityEngine;
using System.Collections;

public class LevelPlanetRotator : MonoBehaviour {

    float selectedRotationSpeed = 30;
    float deselectedRotationSpeed = 10;
    bool selected = false;
    float currentRotationSpeed;

	// Use this for initialization
	void Start () {
        currentRotationSpeed = selected ? selectedRotationSpeed : deselectedRotationSpeed;
	}
	
	// Update is called once per frame
	void Update () {
        currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, selected ? selectedRotationSpeed : deselectedRotationSpeed, Time.deltaTime / 0.1f);
		transform.RotateAround (transform.parent.position, transform.parent.up, currentRotationSpeed * Time.deltaTime);
	}

    public void Select()
    {
        selected = true;
    }

    public void Deselect()
    {
        selected = false;
    }
}
