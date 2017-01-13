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
    void Update() {
        if (rotating)
        {
            // Get rotation delta
            float previousAngle = rotationCurve.Evaluate(rotationValue / rotationTime);
            rotationValue += Time.deltaTime;
            if (rotationValue > 1)
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
			if (Input.GetKeyDown("right") || MobileInput.GetSwipedLeft())
            {
                RotateToNextObject();
            }
			else if (Input.GetKeyDown("left") || MobileInput.GetSwipedRight())
            {
                RotateToPreviousObject();
            }
			else if (Input.GetKeyDown("space") || Input.GetKeyDown("enter") || Input.GetMouseButtonDown(0) || MobileInput.GetTouchUp())
            {
                menuOptions[selectedMenuOption].PerformAction();
            }
        }
	}

    void RotateToNextObject()
    {
        RotateToObjectWithSign(1);
    }

    void RotateToPreviousObject()
    {
        RotateToObjectWithSign(-1);
    }

    void RotateToObjectWithSign(int sign)
    {
        // Deselect old object
        LevelPlanetRotator oldPlanetRotator = menuOptions[selectedMenuOption].GetComponent<LevelPlanetRotator>();
        if (oldPlanetRotator)
            oldPlanetRotator.Deselect();

        // Find the desired rotation delta
        selectedMenuOption = selectedMenuOption + sign*1;
        if (selectedMenuOption >= menuOptions.Length)
            selectedMenuOption -= menuOptions.Length;
        else if (selectedMenuOption < 0)
            selectedMenuOption += menuOptions.Length;

        Vector3 startVector = Vector3.ProjectOnPlane(Camera.main.transform.position - transform.position, transform.up);
        Vector3 endVector = Vector3.ProjectOnPlane(menuOptions[selectedMenuOption].transform.position - transform.position, transform.up);
        totalDesiredRotationDelta = sign*Vector3.Angle(endVector, startVector);

        // Set rotating
        rotating = true;
        rotationValue = 0;

        // Select new object
        LevelPlanetRotator newPlanetRotator = menuOptions[selectedMenuOption].GetComponent<LevelPlanetRotator>();
        if (newPlanetRotator)
            newPlanetRotator.Select();
    }
}
