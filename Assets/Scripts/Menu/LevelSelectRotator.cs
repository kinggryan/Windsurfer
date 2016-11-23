using UnityEngine;
using System.Collections;

public class LevelSelectRotator : MonoBehaviour {

    public MenuOption[] menuOptions;
    public AnimationCurve rotationCurve;
    public float rotationTime;

    private bool rotating;
    private float rotationValue;
    private int selectedMenuOption;
    private float totalDesiredRotationDelta;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(rotating)
        {
            // Get rotation delta
            float previousAngle = rotationCurve.Evaluate(rotationValue / rotationTime);
            rotationValue += Time.deltaTime;
            if(rotationValue > 1)
            {
                rotating = false;
                rotationValue = 1;
            }
            float newAngle = rotationCurve.Evaluate(rotationValue / rotationTime);
            float rotationDelta = (newAngle - previousAngle) * totalDesiredRotationDelta;

            // Rotate around y axis
            transform.Rotate(Vector3.up, rotationDelta, Space.Self);
        }
        else
        {
            if(Input.GetKeyDown("space"))
            {
                RotateToNextObject();
            }
        }
	}

    void RotateToNextObject()
    {
        // Find the desired rotation delta
        selectedMenuOption = (selectedMenuOption + 1) % menuOptions.Length;
        Debug.Log("Selected " + menuOptions[selectedMenuOption]);

        Vector3 startVector = Vector3.Project(Camera.main.transform.position - transform.position, transform.up);
        Vector3 endVector = Vector3.Project(menuOptions[selectedMenuOption].transform.position - transform.position,transform.up);
        totalDesiredRotationDelta = Vector3.Angle(endVector, startVector);

        // Set rotating
        rotating = true;
        rotationValue = 0;
    }
}
