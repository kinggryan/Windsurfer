using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerTrailEffectsManager))]
[RequireComponent(typeof(PlayerUI))]
public class PlayerMouseController : MonoBehaviour {

    [Header("Basic Movement Rates")]
    public float minSpeed = 30f;
    public float maxSpeed = 80f;
    public float brakeRate = 20f;
    public float maxChargeTime = 0.6f;
    public float minChargeTime = 0.3f;
    public float maxBoostSpeed = 20f;
    public float minBoostSpeed = 20f;
    public float minGlideSpeed = 15f;
    public float returnToGlideRate = 5f;
    public Vector3 sphericalMovementVector;

    [Header("Steering Properties")]
    public float maxTurnSpeed;
    public float minTurnInputAngle;
    public float maxTurnInputAngle;
    public float maxChargeTurnSpeed;
    public float maxChargeDirectionAngle;

    [Header("Rain Properties")]
    public float playerRainRadius = 2f;
    public float rainFromChargeDistance = 15f;
    public float rainFromBoostDistance = 15f;
    public float rainFromBoostSpreadDegrees = 30f;
    public float rainFromBoostAmount = 0.75f;
    public float rainMeterGainPerCloud = 0.25f;
    public float rainMeterGainPerForestGrowth = 0.1f;
    public float rainMeterLossPerSecond = 0.25f;
    // The following two properties are not currently used.
    public float rainMeterLossPerSecondCharging = 0.2f;
    public float rainMeterLossPerBoost = 0.33f;
    public float rainlessDamageTime = 4f;

    [Header("Game Object References")]
    public GameObject directionIndicator;

    [Header("Mouse Input")]
    public bool mouseInputMode;
    public float minMouseDistanceFromPlayerToTurnScreenSpace = 160f;

    // Private Properties
    private float speedCharge = 0f;
    private float boostTurnAmount;
    private PlayerTrailEffectsManager trailEffectsManager;
    private PlayerUI ui;
    private float rainMeterAmount = 1f;
    private float rainlessDamageTimer = 0f;

    // Public Methods
    public void ChargeRainMeter(float amount)
    {
        rainMeterAmount = Mathf.Min(1, rainMeterAmount + amount);
    }

    // Private Methods
    void Start()
    {
        trailEffectsManager = GetComponent<PlayerTrailEffectsManager>();
        ui = GetComponent<PlayerUI>();
        trailEffectsManager.StartCharging();
    }

	// Update is called once per frame
	void Update () {
        // Cause rain loss
        bool rainMeterWasAboveZero = rainMeterAmount > 0;
        rainMeterAmount = Mathf.Max(0, rainMeterAmount - Time.deltaTime * rainMeterLossPerSecond);
        if(rainMeterAmount == 0)
        {
            if(rainMeterWasAboveZero)
            {
                rainlessDamageTimer = rainlessDamageTime;
            }
            else
            {
                rainlessDamageTimer -= Time.deltaTime;
                if(rainlessDamageTimer <= 0)
                {
                    Object.FindObjectOfType<PlayerDamageTaker>().TakeDamage(transform.position + Vector3.right);
                    rainlessDamageTimer += rainlessDamageTime;
                }
            }
            trailEffectsManager.SetPlayerRainDamagePercent(1 - (rainlessDamageTimer / rainlessDamageTime));
        }
        else
        {
            trailEffectsManager.SetPlayerRainDamagePercent(0);
        }

        // Update Effects while boosting
        if(Input.GetButtonDown("Boost"))
        {
    //        trailEffectsManager.StartCharging();
        }

        RainFromCharge(rainFromChargeDistance, playerRainRadius);

        // Slow down and charge
        if (Input.GetButton("Boost"))
        {
            bool previouslyCharged = speedCharge >= maxChargeTime;
            speedCharge += Time.deltaTime;
        //    RainFromCharge(rainFromChargeDistance, playerRainRadius);
            if (!previouslyCharged && speedCharge >= maxChargeTime)
            {
                trailEffectsManager.BoostReady();
            }
        }

        // return to glide
        if (sphericalMovementVector.magnitude > minGlideSpeed)
        {
            sphericalMovementVector = sphericalMovementVector.normalized * Mathf.Max(minGlideSpeed, sphericalMovementVector.magnitude - Time.deltaTime * returnToGlideRate);
        }

        // Get input
        Vector3 movementDirectionScreenSpace = Camera.main.transform.InverseTransformDirection(Vector3.Cross(sphericalMovementVector, transform.position)).normalized;
        Vector3 inputDirection = Vector3.zero;
        if (mouseInputMode)
        {
            Vector3 positionScreenSpace = Camera.main.WorldToScreenPoint(transform.position);
            movementDirectionScreenSpace.z = 0;
            inputDirection = (positionScreenSpace - Input.mousePosition) / minMouseDistanceFromPlayerToTurnScreenSpace;
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
      //  Brake();

        // Push to above min speed
        if(sphericalMovementVector.magnitude < minSpeed)
        {
            sphericalMovementVector = minSpeed * sphericalMovementVector.normalized;
        }

        // Move
        transform.RotateAround(Vector3.zero, sphericalMovementVector, sphericalMovementVector.magnitude * Time.deltaTime);

        // Boost
        if(Input.GetButtonUp("Boost"))
        {
            if (speedCharge >= maxChargeTime)
            {
                Quaternion boostDirectionTurn = Quaternion.AngleAxis(boostTurnAmount, transform.position);

                sphericalMovementVector = (Mathf.Min(1f, speedCharge / maxChargeTime) * (maxBoostSpeed - minBoostSpeed) + minBoostSpeed) * (boostDirectionTurn * sphericalMovementVector.normalized);
                boostTurnAmount = 0;
                if (sphericalMovementVector.magnitude > maxSpeed)
                {
                    sphericalMovementVector = sphericalMovementVector.normalized * maxSpeed;
                }
            }
            speedCharge = 0f;
       //     trailEffectsManager.StopCharging();
            RainFromBoost(rainFromBoostDistance, playerRainRadius, rainFromBoostSpreadDegrees);
        }

        // Look
        Vector3 lookDirection = Quaternion.AngleAxis(boostTurnAmount, transform.position) * sphericalMovementVector;
        directionIndicator.transform.rotation = Quaternion.LookRotation(lookDirection, transform.position.normalized);

        // Update Visual Components
        trailEffectsManager.UpdateMovement(sphericalMovementVector, lookDirection,maxChargeDirectionAngle);
        trailEffectsManager.UpdateSpeed((sphericalMovementVector.magnitude - minSpeed) / (maxSpeed - minSpeed));
        trailEffectsManager.UpdateRainMeter(rainMeterAmount);
        ui.UpdateRainMeter(rainMeterAmount);
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

    /// <summary>
    /// Steers the player as needed based on the given inputs.
    /// </summary>
    /// <param name="inputDirection"></param>
    /// <param name="playerScreenMovementDirection"></param>
    /// <param name="chargingBoost"></param>
    void Steer(Vector3 inputDirection, Vector3 playerScreenMovementDirection, bool chargingBoost)
    {
        boostTurnAmount = boostTurnAmount % 360;
        boostTurnAmount = Mathf.Clamp(boostTurnAmount,-maxChargeDirectionAngle, maxChargeDirectionAngle);
        Vector3 currentBoostDirection = Quaternion.AngleAxis(boostTurnAmount, transform.position) * sphericalMovementVector;
        Vector3 currentBoostDirectionScreenSpace = Camera.main.transform.InverseTransformDirection(Vector3.Cross(currentBoostDirection, transform.position)).normalized;

        if (Vector3.Angle(inputDirection, currentBoostDirectionScreenSpace) >= minTurnInputAngle && inputDirection.magnitude >= 1 && Vector3.Angle(inputDirection, currentBoostDirectionScreenSpace) <= maxTurnInputAngle)
        {
            float turnAmount = (Vector3.Angle(inputDirection, playerScreenMovementDirection) - minTurnInputAngle) / (maxTurnInputAngle - minTurnInputAngle);
            turnAmount = Mathf.Clamp(turnAmount, 0f, 1f);
            float sign = Vector3.Angle(Vector3.Cross(Vector3.forward, playerScreenMovementDirection), inputDirection) <= 90 ? -1 : 1;
            sphericalMovementVector = Quaternion.AngleAxis(sign * Time.deltaTime * turnAmount * maxTurnSpeed, transform.position) * sphericalMovementVector ;

            // Turn the boost angle
            if(chargingBoost)
            {
                currentBoostDirectionScreenSpace.z = 0;
                turnAmount = Vector3.Angle(inputDirection, currentBoostDirection) / minTurnInputAngle;
                turnAmount = Mathf.Clamp(turnAmount, 0f, 1f);
                sign = Vector3.Angle(Vector3.Cross(Vector3.forward, currentBoostDirectionScreenSpace), inputDirection) <= 90 ? -1 : 1;

                boostTurnAmount += Time.deltaTime * turnAmount * sign * maxChargeTurnSpeed;
            }
        }
    }

    /// <summary>
    /// Brakes the player if the player should brake based on inputs.
    /// </summary>
    void Brake()
    {
        if(Input.GetButton("Brake"))
        {
            sphericalMovementVector = (sphericalMovementVector.magnitude - brakeRate * Time.deltaTime) * sphericalMovementVector.normalized;
        }
    }

    /// <summary>
    /// Rains behind the player.
    /// </summary>
    /// <param name="chargeRainDistance"></param>
    /// <param name="rainRadius"></param>
    void RainFromCharge(float chargeRainDistance,float rainRadius)
    {
        if (rainMeterAmount > 0)
        {
            // Scale rain based off of how sharply you are turning.
            float rainMultiplier = 1; //Mathf.Abs(boostTurnAmount / maxChargeDirectionAngle);
           // rainMeterAmount = Mathf.Max(0, rainMeterAmount - rainMeterLossPerSecondCharging*Time.deltaTime);
            foreach (RaycastHit hitInfo in Physics.CapsuleCastAll(transform.position, transform.position + -chargeRainDistance * directionIndicator.transform.forward, rainRadius, -transform.position.normalized, 0.5f * transform.position.magnitude))
            {
                Ground ground = hitInfo.collider.GetComponent<Ground>();
                if (ground)
                {
                    ground.RainedOnAtPoint(hitInfo.point,rainMultiplier);
                }
            }
        }
    }

    /// <summary>
    /// Rains in a cone as the player boosts. This will do a burst rain, rather than an incremental rain.
    /// </summary>
    /// <param name="boostRainDistance"></param>
    /// <param name="rainRadius"></param>
    /// <param name="rainSpreadDegrees"></param>
    void RainFromBoost(float boostRainDistance,float rainRadius,float rainSpreadDegrees)
    {
        if (rainMeterAmount > 0)
        {
            // Drain rain meter
          //  rainMeterAmount = Mathf.Max(0, rainMeterAmount - rainMeterLossPerBoost);

            // Use a hash set to guarantee unique colliders
            HashSet<Collider> hitGrounds = new HashSet<Collider>();

            // Create the end points of our spread
            List<Vector3> endPositions = new List<Vector3>();
            for (float spreadAngle = -0.5f * rainSpreadDegrees; spreadAngle <= rainSpreadDegrees; spreadAngle += 10f)
            {
                Vector3 endPosition = transform.position - boostRainDistance * (Quaternion.AngleAxis(spreadAngle, transform.position.normalized) * directionIndicator.transform.forward);
                endPositions.Add(endPosition);
            }

            // Do all the capsule casts
            foreach (Vector3 endPosition in endPositions)
            {
                foreach (RaycastHit hitInfo in Physics.CapsuleCastAll(transform.position, endPosition, rainRadius, -transform.position.normalized, 0.5f * transform.position.magnitude))
                {
                    hitGrounds.Add(hitInfo.collider);
                }
            }

            // Rain on the colliders
            foreach (Collider col in hitGrounds)
            {
                Ground ground = col.GetComponent<Ground>();
                if (ground)
                {
                    ground.RainedOnBurst(rainFromBoostAmount);
                }
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        Cloud cloud = collider.GetComponent<Cloud>();
        if (cloud && cloud.HitPlayer())
        {
            rainMeterAmount = Mathf.Min(1, rainMeterAmount + rainMeterGainPerCloud);
        }
    }

    public void ForestCreated()
    {
        rainMeterAmount = Mathf.Min(1f, rainMeterAmount + rainMeterGainPerForestGrowth);
    }
}
