using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TrailRenderer))]
[RequireComponent(typeof(AudioSource))]
public class PlayerTrailEffectsManager : MonoBehaviour {

    public Renderer playerRenderer;

    [Header("Camera Speed Parameters")]
    public float maxSpeedCameraDistance = 30f;
    public float minSpeedCameraDistance = 20f;
    public float maxSpeedFOV = 60f;
    public float minSpeedFOV = 50f;

    [Header("Damage Effects")]
    public Color damagedTrailColor;
    public Color undamagedTrailColor;

    [Header("Animations")]
    public GameObject playerModel;
    public float playerMaxTurnRate;
    public float maxTurnTiltAngle = 30f;
    public Gradient playerDriftingGradient;
    public Gradient chargeLoopGradient;
    public float playerDriftingGradientDuration;
    public float chargeLoopGradientDuration;
    private Vector3 previousSphericalMovementVector;
    private float previousTurnRate;
    private Color normalPlayerColor;
    private float playerDriftingColorChangeTime;
    private float playerChargeLoopColorChangeTime;

    [Header("Rain Effects")]
    public AnimationCurve rainEmissionCurve;
    public float chargeRainEmissionRate;
    public float chargingParticleSystemScale;
    public float notChargingParticleSystemScale;
    public float chargingParticleSystemRotationRate;
    public float notChargingParticleSystemRotationRate;
    private float currentParticleSystemRotationRate;

    public Color chargingTrailColor;
    public ParticleSystem rainParticleSystem;
    public ParticleSystem chargeRainParticleSystem;
    public ParticleSystem boostRainParticleSystem;
    public ParticleSystem chargingBoostParticleSystem;

    [Header("Audio")]
    public AudioSource rainOverWaterAudio;
    public AudioSource boostAudio;
    public AudioSource startChargingAudio;
    public AudioSource loopingChargeAudio;

    public float rainOverWaterAudioMaxVolume = 0.5f;

    private TrailRenderer trailRenderer;
    private float rainMeter;
    private bool charging = false;
    private float chargeRainMultiplier;

    public float rainOverWaterAudioMaxTurnTargetVolume;
    public float rainOverWaterAudioMinTurnTargetVolume;

    [Header("UI")]
    public UnityEngine.UI.Image rainLossDamageOverlay;
    private float rainLossDamageOverlayMaxOpacity;

    [Header("Boost Flicker")]
    public Color boostFlickerColor;
    public float boostFlickerDuration;
    public float boostFlickerPeriod;

	// Use this for initialization
	void Start () {
        trailRenderer = GetComponent<TrailRenderer>();
        chargingBoostParticleSystem.Play();
        chargingBoostParticleSystem.transform.localScale = notChargingParticleSystemScale * (new Vector3(1, 1, 1));
        rainOverWaterAudio.volume = 0f;
        rainLossDamageOverlayMaxOpacity = rainLossDamageOverlay.color.a;
        normalPlayerColor = playerModel.GetComponent<Renderer>().material.color;
        playerDriftingColorChangeTime = 0;
        playerChargeLoopColorChangeTime = 0;
        currentParticleSystemRotationRate = notChargingParticleSystemRotationRate;
        charging = false;
    }

    // Update is called once per frame
    void Update () {
        // Update the scale of the charge/rain particle system
        float targetScale = charging ? chargingParticleSystemScale : notChargingParticleSystemScale;
        chargingBoostParticleSystem.transform.localScale = Mathf.Lerp(chargingBoostParticleSystem.transform.localScale.x,targetScale, 8 * Time.deltaTime) * (new Vector3(1, 1, 1));

        currentParticleSystemRotationRate = Mathf.Lerp(currentParticleSystemRotationRate, charging ? chargingParticleSystemRotationRate : notChargingParticleSystemRotationRate, 8*Time.deltaTime);
        rainParticleSystem.GetComponent<LocalRotator>().rotationRate = currentParticleSystemRotationRate;

        if (playerDriftingColorChangeTime > 0)
        {
            playerDriftingColorChangeTime -= Time.deltaTime;
            Color color = playerDriftingColorChangeTime > 0 ? playerDriftingGradient.Evaluate(1 - (playerDriftingColorChangeTime / playerDriftingGradientDuration)) : normalPlayerColor;
            playerModel.GetComponent<Renderer>().material.color = color;
            if(playerDriftingColorChangeTime <= 0)
            {
                playerChargeLoopColorChangeTime = chargeLoopGradientDuration;
            }
        }
        else if(playerChargeLoopColorChangeTime > 0)
        {
            playerChargeLoopColorChangeTime -= Time.deltaTime;
            if (charging)
            {
                // If charging, loop and set the color
                if (playerChargeLoopColorChangeTime < 0)
                    playerChargeLoopColorChangeTime += chargeLoopGradientDuration;
                Color color = chargeLoopGradient.Evaluate(1 - (playerChargeLoopColorChangeTime / chargeLoopGradientDuration));
                playerModel.GetComponent<Renderer>().material.color = color;
            }
            else
            {
                // If not charging, fade back to normal player color.
                playerChargeLoopColorChangeTime = 0f;
                playerModel.GetComponent<Renderer>().material.color = normalPlayerColor;
            }
        }
	}

    public void StartCharging()
    {
        charging = true; 
        if (rainMeter > 0)
        {
            chargeRainParticleSystem.Play();
            startChargingAudio.Play();
        //    chargingBoostParticleSystem.Play();
        }
        playerDriftingColorChangeTime = playerDriftingGradientDuration;
    }

    public void StopCharging()
    {
        charging = false;
        chargeRainParticleSystem.Stop();
        if (rainMeter > 0)
        {
            boostRainParticleSystem.Play();
            boostAudio.Play();
        }

    //    chargingBoostParticleSystem.Stop();
    }

    public void StartBraking()
    {

    }

    public void StopBraking()
    {

    }

    public void UpdateMovement(Vector3 playerSphericalMotionVector,Vector3 playerLookDirection,float playerMaxTurnDirection)
    {
        chargeRainMultiplier = Vector3.Angle(playerSphericalMotionVector, playerLookDirection) / playerMaxTurnDirection;

        float sign = Vector3.Angle(Vector3.Cross(playerSphericalMotionVector, transform.position), previousSphericalMovementVector) <= 90 ? -1 : 1;
        float turnRate = Vector3.Angle(playerSphericalMotionVector, previousSphericalMovementVector) / Time.deltaTime;

        // Debug.Log(Vector3.Angle(playerSphericalMotionVector, previousSphericalMovementVector));
        //Debug.Log(playerSphericalMotionVector);
        float visibleTurnRate = Mathf.Lerp(previousTurnRate, turnRate, 0.5f);
        visibleTurnRate = Mathf.MoveTowards(visibleTurnRate, 0, 15);
        visibleTurnRate *= 1.5f;
       // Debug.Log(visibleTurnRate);
      // Debug.Log((playerSphericalMotionVector - previousSphericalMovementVector).magnitude);

        Vector3 newEulerAngles = playerModel.transform.localEulerAngles;
        newEulerAngles.y = 90 + sign * visibleTurnRate / playerMaxTurnRate * maxTurnTiltAngle;
        playerModel.transform.localEulerAngles = newEulerAngles;

        previousSphericalMovementVector = playerSphericalMotionVector;
        previousTurnRate = turnRate;
   /*     float sign = Vector3.Angle(Vector3.Cross(playerSphericalMotionVector, transform.position), playerLookDirection) <= 90 ? -1 : 1;
        ParticleSystem.VelocityOverLifetimeModule vm = chargeRainParticleSystem.velocityOverLifetime;
        float min = Mathf.Min(sign * Vector3.Angle(playerSphericalMotionVector, playerLookDirection),0);
        float max = Mathf.Max(sign * Vector3.Angle(playerSphericalMotionVector, playerLookDirection), 0);
        vm.x = new ParticleSystem.MinMaxCurve(min, max); */
        //chargeRainParticleSystem.transform.localRotation = Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(-90, Vector3.forward);
    }

    public void UpdateSpeed(float speedToMaxSpeedRatio)
    {
       // Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition,new Vector3(Camera.main.transform.localPosition.x,Camera.main.transform.localPosition.y,minSpeedCameraDistance + (maxSpeedCameraDistance - minSpeedCameraDistance) * speedToMaxSpeedRatio),0.5f);
       // Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView,minSpeedFOV + (maxSpeedFOV - minSpeedFOV) * speedToMaxSpeedRatio,0.5f);
    }

    public void UpdateRainMeter(float rainMeter)
    {
        this.rainMeter = rainMeter;

        ParticleSystem.EmissionModule emission = rainParticleSystem.emission;

        if (rainMeter > 0)
        {
            emission.rate = new ParticleSystem.MinMaxCurve(rainEmissionCurve.Evaluate(rainMeter));
            emission.enabled = true;
            if (charging)
            {
                emission = chargeRainParticleSystem.emission;
                if (!emission.enabled)
                {
                    emission.enabled = true;
                }
                emission.rate = new ParticleSystem.MinMaxCurve(chargeRainMultiplier * chargeRainEmissionRate);

                var rainVolume = chargeRainMultiplier * (rainOverWaterAudioMaxTurnTargetVolume - rainOverWaterAudioMinTurnTargetVolume) + rainOverWaterAudioMinTurnTargetVolume;
                rainOverWaterAudio.volume = Mathf.MoveTowards(rainOverWaterAudio.volume, rainVolume, Time.deltaTime*5);
            }
            else
            {
                rainOverWaterAudio.volume = Mathf.MoveTowards(rainOverWaterAudio.volume, 0, Time.deltaTime*5);
            }
        }
        else
        {
            emission.enabled = false;
            if (charging && chargeRainParticleSystem.emission.enabled)
            {
                emission = chargeRainParticleSystem.emission;
                emission.enabled = false;
            }
            rainOverWaterAudio.volume = Mathf.MoveTowards(rainOverWaterAudio.volume, 0, Time.deltaTime * 5);
        }
    }

    public void BoostReady()
    {
        StartCoroutine(FlickerBoostColor(boostFlickerDuration));
    //    Debug.Log("Boost ready" + Time.time);
    }

    public IEnumerator FlickerBoostColor(float totalTimeLeft)
    {
        yield return new WaitForSeconds(boostFlickerPeriod);

     //   Color currentColor = playerRenderer.material.color;
    //    if(currentColor == boostFlickerColor || totalTimeLeft <= 0)
    //    {
        //    playerRenderer.material.color = Color.white;
    //    }
   //     else
    //    {
         //   playerRenderer.material.color = Color.white; //boostFlickerColor;
  //      }

        if(totalTimeLeft > 0)
            StartCoroutine(FlickerBoostColor(totalTimeLeft - boostFlickerPeriod));
    }

    public void SetPlayerRainDamagePercent(float rainDamageTimerPercent)
    {
        // 1.0 is taking damage, 0 is not at all.
        Color color = rainLossDamageOverlay.color;
        color.a = rainDamageTimerPercent * rainLossDamageOverlayMaxOpacity;
  //      rainLossDamageOverlay.color = color;
        if(rainDamageTimerPercent == 0)
        {
            trailRenderer.material.color = undamagedTrailColor;
        }
        else {
            trailRenderer.material.color = damagedTrailColor;
        }
    }
}
