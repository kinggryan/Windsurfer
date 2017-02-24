using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerTrailEffectsManager))]
[RequireComponent(typeof(PlayerUI))]
[RequireComponent(typeof(PlayerDamageFlicker))]
public class PlayerMouseController : MonoBehaviour {

    [Header("Basic Movement Rates")]
    public float minSpeed = 30f;
    public float maxSpeed = 80f;
    public float maxChargeTime = 0.6f;
    public float maxBoostSpeed = 20f;
    public float minBoostSpeed = 20f;
    public float minGlideSpeed = 15f;
    public float returnToGlideRate = 5f;
    public Vector3 sphericalMovementVector;
    public float chargingRumbleIntensity;
    public float boostCameraShakeIntensity;

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
    public float rainlessDamageTime = 4f;
	public float damageAmount = 0.3f;

    [Header("Game Object References")]
    public GameObject directionIndicator;

	[Header("Mobile Input")]
	public bool mobileInputMode;
	public float minTouchDistanceFromPlayerToTurnScreenSpace = 20f;

    [Header("Mouse Input")]
    public bool mouseInputMode;
    public float minMouseDistanceFromPlayerToTurnScreenSpace = 160f;

    // Private Properties
    private float speedCharge = 0f;
    private float boostTurnAmount;
    private PlayerTrailEffectsManager trailEffectsManager;
    private PlayerUI ui;
    private PlayerDamageFlicker flickerer;
    private bool damaged = false;
    private float rainlessDamageTimer = 0f;
	private Vector3 previousInputMovementDirection = Vector3.up;

    // Private Methods
    void Start()
    {
        trailEffectsManager = GetComponent<PlayerTrailEffectsManager>();
        ui = GetComponent<PlayerUI>();
        flickerer = GetComponent<PlayerDamageFlicker>();
        flickerer.deathDuration = rainlessDamageTime;
        trailEffectsManager.StartCharging();

        sphericalMovementVector = minGlideSpeed * sphericalMovementVector.normalized;

		// Automatically go into mobile input mode if touch is supported
		mobileInputMode = Input.touchSupported;
    }

	void ReduceTimerIfDamaged()
	{
		// Cause rain loss if the rain meter 
		if(damaged)
		{
			rainlessDamageTimer -= Time.deltaTime;
			if (rainlessDamageTimer <= 0)
			{
				Object.FindObjectOfType<PlayerDamageTaker>().RainRanOut();
				rainlessDamageTimer = 0;
				flickerer.PlayerDied();
			}
			trailEffectsManager.SetPlayerRainDamagePercent(1 - (rainlessDamageTimer / rainlessDamageTime));
			ui.UpdateRainTimer (rainlessDamageTimer / rainlessDamageTime);
		}
		else
		{
			trailEffectsManager.SetPlayerRainDamagePercent(0);
			ui.UpdateRainTimer (1);
		}
	}

	// Update is called once per frame
	void Update () {
		ReduceTimerIfDamaged ();

        // Update Effects while boosting
		if(mobileInputMode ? MobileInput.GetTouchDown() : Input.GetButtonDown("Boost"))
        {
            trailEffectsManager.StartCharging();
        }

        // Slow down and charge
		if (mobileInputMode ? MobileInput.GetTouched() : Input.GetButton("Boost"))
        {
            bool previouslyCharged = speedCharge >= maxChargeTime;
            speedCharge += Time.deltaTime;
            RainFromCharge(rainFromChargeDistance, playerRainRadius);
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
		if (mobileInputMode) {
			if (Input.touchCount > 0) {
				// flatten screen space position
				Vector3 positionScreenSpace = Camera.main.WorldToScreenPoint (transform.position);
				movementDirectionScreenSpace.z = 0;

				// Convert touch to 3d vector
				Vector3 touchPositionScreenSpace = new Vector3 (Input.touches [0].position.x, Input.touches [0].position.y,0);
				inputDirection = (positionScreenSpace - touchPositionScreenSpace) / minTouchDistanceFromPlayerToTurnScreenSpace;
				inputDirection *= -1;
				previousInputMovementDirection = inputDirection;
			} else {
				inputDirection = previousInputMovementDirection;
			}
		}
        else if (mouseInputMode)
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
		inputDirection.z = 0;
		Steer( inputDirection, movementDirectionScreenSpace, mobileInputMode ? MobileInput.GetTouched() : Input.GetButton("Boost"));

        // Push to above min speed
        if(sphericalMovementVector.magnitude < minSpeed)
        {
            sphericalMovementVector = minSpeed * sphericalMovementVector.normalized;
        }

        // Move
        transform.RotateAround(Vector3.zero, sphericalMovementVector, sphericalMovementVector.magnitude * Time.deltaTime);

        // Boost
		if(mobileInputMode ? MobileInput.GetTouchUp() : Input.GetButtonUp("Boost"))
        {
			// If we've been charging long enough
            if (speedCharge >= maxChargeTime)
            {
                Quaternion boostDirectionTurn = Quaternion.AngleAxis(boostTurnAmount, transform.position);

                sphericalMovementVector = (Mathf.Min(1f, speedCharge / maxChargeTime) * (maxBoostSpeed - minBoostSpeed) + minBoostSpeed) * (boostDirectionTurn * sphericalMovementVector.normalized);
                if (sphericalMovementVector.magnitude > maxSpeed)
                {
					sphericalMovementVector = sphericalMovementVector.normalized * maxSpeed;
                }

                // Shake camera
                Vector3 shakeDirection = Camera.main.transform.InverseTransformDirection(Vector3.Cross(sphericalMovementVector, transform.position));
                shakeDirection.z = 0;
                shakeDirection.Normalize();
                Camera.main.GetComponent<CameraShaker>().ShakeInDirectionWithIntensity(shakeDirection, boostCameraShakeIntensity, CameraShaker.ShakeType.Smooth);
            	
				trailEffectsManager.Boost ();
				RainFromBoost(rainFromBoostDistance, playerRainRadius, rainFromBoostSpreadDegrees);
			}

			boostTurnAmount = 0;
            speedCharge = 0f;
            trailEffectsManager.StopCharging();
        }

        // Look
        Vector3 lookDirection = Quaternion.AngleAxis(boostTurnAmount, transform.position) * sphericalMovementVector;
        directionIndicator.transform.rotation = Quaternion.LookRotation(lookDirection, transform.position.normalized);

        // Update Visual Components
        trailEffectsManager.UpdateMovement(sphericalMovementVector, lookDirection,maxChargeDirectionAngle);
        trailEffectsManager.UpdateSpeed((sphericalMovementVector.magnitude - minSpeed) / (maxSpeed - minSpeed));
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

		if (Vector3.Angle(inputDirection, playerScreenMovementDirection) >= minTurnInputAngle && inputDirection.magnitude >= 1 )
        {
            float turnAmount = (Vector3.Angle(inputDirection, playerScreenMovementDirection) - minTurnInputAngle) / (maxTurnInputAngle - minTurnInputAngle);
            turnAmount = Mathf.Clamp(turnAmount, 0f, 1f);
			float sign = Vector3.Angle(Vector3.Cross(Vector3.forward, playerScreenMovementDirection), inputDirection) <= 90 ? -1 : 1;
            sphericalMovementVector = Quaternion.AngleAxis(sign * Time.deltaTime * turnAmount * maxTurnSpeed, transform.position) * sphericalMovementVector ;

			if(chargingBoost)
				boostTurnAmount = sign * turnAmount * maxChargeDirectionAngle;
        }
    }

    /// <summary>
    /// Rains behind the player.
    /// </summary>
    /// <param name="chargeRainDistance"></param>
    /// <param name="rainRadius"></param>
    void RainFromCharge(float chargeRainDistance,float rainRadius)
    {
        // Scale rain based off of how sharply you are turning.
        float rainMultiplier = Mathf.Abs(boostTurnAmount / maxChargeDirectionAngle);
        foreach (RaycastHit hitInfo in Physics.CapsuleCastAll(transform.position, transform.position + -chargeRainDistance * directionIndicator.transform.forward, rainRadius, -transform.position.normalized, 0.5f * transform.position.magnitude))
        {
            Ground ground = hitInfo.collider.GetComponent<Ground>();
            if (ground)
            {
                ground.RainedOnAtPoint(hitInfo.point,rainMultiplier);
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

    public void ForestCreated()
    {
		Debug.Log ("Increasing rainless damage timer...");
		rainlessDamageTimer = Mathf.Min (rainlessDamageTime, rainlessDamageTimer + 0.34f * rainlessDamageTime);
		if (rainlessDamageTimer >= rainlessDamageTime) {
			damaged = false;
			flickerer.Heal ();
		}
    }

    public void TakeDamageAndLoseRain()
    {
		if (!damaged)
        {
			damaged = true;
            rainlessDamageTimer = rainlessDamageTime;
        }
        else
        {
			rainlessDamageTimer -= damageAmount*rainlessDamageTime;
			if (rainlessDamageTimer <= 0) {
				// Die
				Object.FindObjectOfType<PlayerDamageTaker> ().RainRanOut ();
			}
        }
    }

	public void Bounce() {
		// Test
		sphericalMovementVector *= -1;
	}
}
