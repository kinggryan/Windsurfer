using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerTrailEffectsManager))]
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

    public float playerRainRadius = 2f;
    public float rainWhileDriftDistance = 15f;
    public float rainFromBoostDistance = 15f;
    public float rainFromBoostSpreadDegrees = 30f;

    public GameObject windGustPrefab;
    public GameObject directionIndicator;
    public Renderer rainFromChargeIndicator;

    public bool mouseInputMode;

    private float speedCharge = 0f;

    private float boostTurnAmount;

    private PlayerTrailEffectsManager trailEffectsManager;

    void Start()
    {
        trailEffectsManager = GetComponent<PlayerTrailEffectsManager>();
        rainFromChargeIndicator.enabled = false;
    }

	// Update is called once per frame
	void Update () {
        if(Input.GetButtonDown("Boost"))
        {
            trailEffectsManager.StartCharging();
            rainFromChargeIndicator.enabled = true;
        }

        // Slow down and charge
        if (Input.GetButton("Boost"))
        {
            speedCharge += Time.deltaTime;

            RainFromCharge(rainWhileDriftDistance, playerRainRadius);
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
            trailEffectsManager.StopCharging();
            rainFromChargeIndicator.enabled = false;
            RainFromBoost(rainFromBoostDistance, playerRainRadius, rainFromBoostSpreadDegrees);
        }

        // Look
        Vector3 lookDirection = Quaternion.AngleAxis(boostTurnAmount, transform.position) * sphericalMovementVector;
        directionIndicator.transform.rotation = Quaternion.LookRotation(lookDirection, transform.position.normalized);

        // Rain
       // RainBeneathPlayer(playerRainRadius);
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

    void RainBeneathPlayer(float rainRadius)
    {   
        foreach (RaycastHit hitInfo in Physics.SphereCastAll(transform.position, rainRadius, -1 * transform.position, 0.5f * transform.position.magnitude))
        {
            Ground ground = hitInfo.collider.GetComponent<Ground>();
            if (ground)
            {
                ground.RainedOnAtPoint(hitInfo.point);
            }
        }
    }

    void RainFromCharge(float chargeRainDistance,float rainRadius)
    {
        foreach (RaycastHit hitInfo in Physics.CapsuleCastAll(transform.position, transform.position + -chargeRainDistance * directionIndicator.transform.forward, rainRadius, -transform.position.normalized,0.5f*transform.position.magnitude))
        {
            Ground ground = hitInfo.collider.GetComponent<Ground>();
            if (ground)
            {
                ground.RainedOnAtPoint(hitInfo.point);
            }
        }
    }

    void RainFromBoost(float boostRainDistance,float rainRadius,float rainSpreadDegrees)
    {
        // Use a hash set to guarantee unique colliders
        HashSet<Collider> hitGrounds = new HashSet<Collider>();

        // Create the end points of our spread
        List<Vector3> endPositions = new List<Vector3>();
        for(float spreadAngle = -0.5f*rainSpreadDegrees; spreadAngle <= rainSpreadDegrees; spreadAngle += 10f)
        {
            Vector3 endPosition = transform.position - boostRainDistance * (Quaternion.AngleAxis(spreadAngle,transform.position.normalized) * directionIndicator.transform.forward);
            endPositions.Add(endPosition);
        }

        // Do all the capsule casts
        foreach(Vector3 endPosition in endPositions)
        {
            foreach (RaycastHit hitInfo in Physics.CapsuleCastAll(transform.position, endPosition, rainRadius, -transform.position.normalized, 0.5f * transform.position.magnitude))
            {
                hitGrounds.Add(hitInfo.collider);
            }
        }
        
        // Rain on the colliders
        foreach(Collider col in hitGrounds)
        {
            Ground ground = col.GetComponent<Ground>();
            if (ground)
            {
                Debug.Log("Thang");
                ground.RainedOnBurst(0.75f);
            }
        }
    }
}
