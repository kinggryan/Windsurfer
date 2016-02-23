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

    private float speedCharge = 0f;

    private float boostTurnAmount;

	// Update is called once per frame
	void Update () {
        // Slow down and charge
        if (Input.GetMouseButton(0))
        {
            speedCharge += Time.deltaTime;
         //   sphericalMovementVector = sphericalMovementVector.normalized*Mathf.Max(0, sphericalMovementVector.magnitude - Time.deltaTime * maxSpeed / fullSpeedBrakeTime);
        }
        if (sphericalMovementVector.magnitude > minGlideSpeed)
        {
            sphericalMovementVector = sphericalMovementVector.normalized * Mathf.Max(minGlideSpeed, sphericalMovementVector.magnitude - Time.deltaTime * maxSpeed / fullSpeedBrakeTime);
        }

        // Steer
        Vector3 positionScreenSpace = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 movementDirectionScreenSpace = Camera.main.transform.InverseTransformDirection(Vector3.Cross(sphericalMovementVector, transform.position)).normalized;
        movementDirectionScreenSpace.z = 0;
        Steer(Input.mousePosition, positionScreenSpace, movementDirectionScreenSpace,Input.GetMouseButton(0));
        Brake(Input.mousePosition, positionScreenSpace, movementDirectionScreenSpace);

        // Move
        transform.RotateAround(Vector3.zero, sphericalMovementVector, sphericalMovementVector.magnitude * Time.deltaTime);

        // Boost
        if(Input.GetMouseButtonUp(0))
        {
            if (speedCharge >= minChargeTime)
            {
             //   Vector3 inputSphericalMovement = -1 * PlayerReleasedMouseAtScreenPosition(Input.mousePosition);
                //sphericalMovementVector += inputSphericalMovement;
                Quaternion boostDirectionTurn = Quaternion.AngleAxis(boostTurnAmount, transform.position);

                sphericalMovementVector = (Mathf.Min(1f, speedCharge / maxSpeedChargeTime) * (additionalSpeedOnClick - minAdditionalSpeedOnClick) + minAdditionalSpeedOnClick) * (boostDirectionTurn * sphericalMovementVector.normalized);
                boostTurnAmount = 0;
                if (sphericalMovementVector.magnitude > maxSpeed)
                {
                    sphericalMovementVector = sphericalMovementVector.normalized * maxSpeed;
                }

                SendStorm(GetWorldSpaceVectorFromMouseToPlayer(Input.mousePosition), 30f, -1 * sphericalMovementVector);
            }
            speedCharge = 0f;
        }

        // Look
        Vector3 lookDirection = Quaternion.AngleAxis(boostTurnAmount, transform.position) * sphericalMovementVector;
        directionIndicator.transform.rotation = Quaternion.LookRotation(lookDirection, transform.position.normalized);
    }

    Vector3 GetWorldSpaceVectorFromMouseToPlayer(Vector3 mouseScreenPosition)
    {
        Vector3 positionScreenSpace = Camera.main.WorldToScreenPoint(transform.position);
        positionScreenSpace.z = 0;
        Vector3 movementDirectionScreenSpace = positionScreenSpace - mouseScreenPosition;
        return Camera.main.transform.TransformDirection(movementDirectionScreenSpace);
    }

    Vector3 PlayerReleasedMouseAtScreenPosition(Vector3 mouseScreenPosition)
    {
        Vector3 movementDirectionWorldSpace = GetWorldSpaceVectorFromMouseToPlayer(mouseScreenPosition);
        Vector3 newRotationAxis = -1*Vector3.Cross(Camera.main.transform.forward, movementDirectionWorldSpace);

        return (Mathf.Min(1f,speedCharge/maxSpeedChargeTime)*(additionalSpeedOnClick-minAdditionalSpeedOnClick) + minAdditionalSpeedOnClick) * newRotationAxis.normalized; 
    }

    void SendStorm(Vector3 stormDirection,float stormDistance,Vector3 sphericalVelocity)
    {
        PlayerWindGust gust = ((GameObject)GameObject.Instantiate(windGustPrefab, transform.position, Quaternion.LookRotation(Vector3.Cross(Vector3.up,transform.position),transform.position))).GetComponent<PlayerWindGust>();
        gust.LaunchWithSphericalVelocityAndLifespan(sphericalVelocity, stormDistance / sphericalVelocity.magnitude);

  /*      RaycastHit hitInfo;
        if(Physics.Raycast(transform.position,stormDirection,out hitInfo, stormDistance))
        {
            Debug.Log("hit");
            SphereSurfaceSlider slider = hitInfo.collider.GetComponent<SphereSurfaceSlider>();
            if(slider)
            {
                slider.HitByStormWithSphericalVelocity(sphericalVelocity);
            }
        } */
    }

    void Steer(Vector3 mousePosition, Vector3 playerScreenPosition, Vector3 playerScreenMovementDirection, bool chargingBoost)
    {
        Vector3 playerToMouseScreenVector = mousePosition - playerScreenPosition;
        playerToMouseScreenVector.z = 0;
       // Debug.Log(Vector3.Angle(playerToMouseScreenVector, playerScreenMovementDirection));
        if (Vector3.Angle(playerToMouseScreenVector, playerScreenMovementDirection) >= minTurnMouseMovementVectorAngle && Vector3.Distance(mousePosition, playerScreenPosition) >= minDistanceForTurning && Vector3.Angle(playerToMouseScreenVector, playerScreenMovementDirection) <= maxTurnMouseMovementVectorAngle)
        {
            float turnAmount = (Vector3.Angle(playerToMouseScreenVector, playerScreenMovementDirection) - minTurnMouseMovementVectorAngle) / (maxTurnMouseMovementVectorAngle - minTurnMouseMovementVectorAngle);
            turnAmount = Mathf.Clamp(turnAmount, 0f, 1f);
            float sign = Vector3.Angle(Vector3.Cross(Vector3.forward, playerScreenMovementDirection), playerToMouseScreenVector) <= 90 ? -1 : 1;
            sphericalMovementVector = Quaternion.AngleAxis(sign * Time.deltaTime * turnAmount * maxTurnSpeed, transform.position) * sphericalMovementVector ;

            // Turn the boost angle
            if(chargingBoost)
            {
                Vector3 currentBoostDirection = Quaternion.AngleAxis(boostTurnAmount, transform.position) * sphericalMovementVector;
                Vector3 currentBoostDirectionScreenSpace = Camera.main.transform.InverseTransformDirection(Vector3.Cross(currentBoostDirection, transform.position)).normalized;
                currentBoostDirectionScreenSpace.z = 0;
                turnAmount = Vector3.Angle(playerToMouseScreenVector, currentBoostDirection) / maxTurnMouseMovementVectorAngle;
                turnAmount = Mathf.Clamp(turnAmount, 0f, 1f);
                sign = Vector3.Angle(Vector3.Cross(Vector3.forward, currentBoostDirectionScreenSpace), playerToMouseScreenVector) <= 90 ? -1 : 1;

                boostTurnAmount += Time.deltaTime * turnAmount * sign * maxBoostTurnSpeed;
              //  Debug.Log(Time.time + " " + boostTurnAmount);
            }
        }
    }

    void Brake(Vector3 mousePosition, Vector3 playerScreenPosition, Vector3 playerScreenMovementDirection)
    {
        Vector3 playerToMouseScreenVector = mousePosition - playerScreenPosition;
        playerToMouseScreenVector.z = 0;
        if(Vector3.Angle(playerToMouseScreenVector, playerScreenMovementDirection) >= maxTurnMouseMovementVectorAngle)
        {
            sphericalMovementVector = (sphericalMovementVector.magnitude - brakeSpeed * Time.deltaTime) * sphericalMovementVector.normalized;
        }
    }
}
