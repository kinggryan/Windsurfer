using UnityEngine;
using System.Collections;

public class PlayerMouseController : MonoBehaviour {

    public float fullSpeedBrakeTime = 1.0f;
    public float maxSpeed = 80f;
    public float maxSpeedChargeTime = 0.6f;
    public float minChargeTime = 0.3f;
    public float additionalSpeedOnClick = 20f;
    public float minAdditionalSpeedOnClick = 20f;
    public Vector3 sphericalMovementVector;

    public float maxTurnSpeed;
    public float minTurnMouseMovementVectorAngle;
    public float maxTurnMouseMovementVectorAngle;
    public float minDistanceForTurning;
    public float maxDistanceForTurning = 160f;

    public float brakeSpeed = 10f;

    public float minGlideSpeed = 15f;

    public float maxBoostTurnSpeed;

    public GameObject windGustPrefab;
    public GameObject directionIndicator;

    public bool mouseInputMode;

    private float speedCharge = 0f;

    private float boostTurnAmount;

	// Update is called once per frame
	void Update () {
        // Slow down and charge
        if (Input.GetButton("Boost"))
        {
            speedCharge += Time.deltaTime;
        }

        if (sphericalMovementVector.magnitude > minGlideSpeed)
        {
            sphericalMovementVector = sphericalMovementVector.normalized * Mathf.Max(minGlideSpeed, sphericalMovementVector.magnitude - Time.deltaTime * maxSpeed / fullSpeedBrakeTime);
        }

        // Get input
        Vector3 movementDirectionScreenSpace = Camera.main.transform.InverseTransformDirection(Vector3.Cross(sphericalMovementVector, transform.position)).normalized;
        Vector3 inputDirection = Vector3.zero;
        if (mouseInputMode)
        {
            Vector3 positionScreenSpace = Camera.main.WorldToScreenPoint(transform.position);
            movementDirectionScreenSpace.z = 0;
            inputDirection = (positionScreenSpace - Input.mousePosition) / maxDistanceForTurning;
            inputDirection *= -1;
        }
        else
        {
            inputDirection = (new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0));
            inputDirection *= 1.5f;
            if(inputDirection.magnitude > 1.0f)
            {
                inputDirection = inputDirection.normalized;
            }
        }

        // Steer and brake
        Steer( inputDirection, movementDirectionScreenSpace, Input.GetButton("Boost"));
        Brake( inputDirection, movementDirectionScreenSpace);

        // Move
        transform.RotateAround(Vector3.zero, sphericalMovementVector, sphericalMovementVector.magnitude * Time.deltaTime);

        // Boost
        if(Input.GetButtonUp("Boost"))
        {
            if (speedCharge >= minChargeTime)
            {
                Quaternion boostDirectionTurn = Quaternion.AngleAxis(boostTurnAmount, transform.position);

                sphericalMovementVector = (Mathf.Min(1f, speedCharge / maxSpeedChargeTime) * (additionalSpeedOnClick - minAdditionalSpeedOnClick) + minAdditionalSpeedOnClick) * (boostDirectionTurn * sphericalMovementVector.normalized);
                boostTurnAmount = 0;
                if (sphericalMovementVector.magnitude > maxSpeed)
                {
                    sphericalMovementVector = sphericalMovementVector.normalized * maxSpeed;
                }

                SendStorm(GetWorldSpaceVectorFromInputVector(inputDirection), 30f, -1 * sphericalMovementVector);
            }
            speedCharge = 0f;
        }

        // Look
        Vector3 lookDirection = Quaternion.AngleAxis(boostTurnAmount, transform.position) * sphericalMovementVector;
        directionIndicator.transform.rotation = Quaternion.LookRotation(lookDirection, transform.position.normalized);
    }

    Vector3 GetWorldSpaceVectorFromInputVector(Vector3 inputVector)
    {
        return Camera.main.transform.TransformDirection(inputVector);
    }

    Vector3 GetWorldSpaceVectorFromMouseToPlayer(Vector3 mouseScreenPosition)
    {
        Vector3 positionScreenSpace = Camera.main.WorldToScreenPoint(transform.position);
        positionScreenSpace.z = 0;
        Vector3 movementDirectionScreenSpace = positionScreenSpace - mouseScreenPosition;
        return Camera.main.transform.TransformDirection(movementDirectionScreenSpace);
    }

    void SendStorm(Vector3 stormDirection,float stormDistance,Vector3 sphericalVelocity)
    {
        PlayerWindGust gust = ((GameObject)GameObject.Instantiate(windGustPrefab, transform.position, Quaternion.LookRotation(Vector3.Cross(Vector3.up,transform.position),transform.position))).GetComponent<PlayerWindGust>();
        gust.LaunchWithSphericalVelocityAndLifespan(sphericalVelocity, stormDistance / sphericalVelocity.magnitude);
    }

    void Steer(Vector3 inputDirection, Vector3 playerScreenMovementDirection, bool chargingBoost)
    {
        Vector3 currentBoostDirection = Quaternion.AngleAxis(boostTurnAmount, transform.position) * sphericalMovementVector;
        Vector3 currentBoostDirectionScreenSpace = Camera.main.transform.InverseTransformDirection(Vector3.Cross(currentBoostDirection, transform.position)).normalized;

        if (Vector3.Angle(inputDirection, currentBoostDirectionScreenSpace) >= minTurnMouseMovementVectorAngle && inputDirection.magnitude >= 1 && Vector3.Angle(inputDirection, currentBoostDirectionScreenSpace) <= maxTurnMouseMovementVectorAngle)
        {
            float turnAmount = (Vector3.Angle(inputDirection, playerScreenMovementDirection) - minTurnMouseMovementVectorAngle) / (maxTurnMouseMovementVectorAngle - minTurnMouseMovementVectorAngle);
            turnAmount = Mathf.Clamp(turnAmount, 0f, 1f);
            float sign = Vector3.Angle(Vector3.Cross(Vector3.forward, playerScreenMovementDirection), inputDirection) <= 90 ? -1 : 1;
            sphericalMovementVector = Quaternion.AngleAxis(sign * Time.deltaTime * turnAmount * maxTurnSpeed, transform.position) * sphericalMovementVector ;

            // Turn the boost angle
            if(chargingBoost)
            {
                currentBoostDirectionScreenSpace.z = 0;
                turnAmount = Vector3.Angle(inputDirection, currentBoostDirection) / maxTurnMouseMovementVectorAngle;
                turnAmount = Mathf.Clamp(turnAmount, 0f, 1f);
                sign = Vector3.Angle(Vector3.Cross(Vector3.forward, currentBoostDirectionScreenSpace), inputDirection) <= 90 ? -1 : 1;

                boostTurnAmount += Time.deltaTime * turnAmount * sign * maxBoostTurnSpeed;
            }
        }
    }

    void Brake(Vector3 inputDirection, Vector3 playerScreenMovementDirection)
    {
        if(Input.GetButton("Brake")) // if(Vector3.Angle(inputDirection, playerScreenMovementDirection) >= maxTurnMouseMovementVectorAngle)
        {
            sphericalMovementVector = (sphericalMovementVector.magnitude - brakeSpeed * Time.deltaTime) * sphericalMovementVector.normalized;
        }
    }
}
